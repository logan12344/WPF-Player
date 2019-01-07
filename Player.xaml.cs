using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.IO;
using System.Windows.Media.Effects;
using System.Globalization;
using System.Windows.Media.Imaging;
using System.Data.SQLite;
using System.Threading;
using System.Threading.Tasks;
using Player;

namespace MusicPlayer
{
    public partial class PlayerWPF : Window
    {
        private List<String> audioList = new List<String>();
        private List<String> pathList = new List<String>();
        private MediaPlayer player = new MediaPlayer();
        private int i = 0, randomNumb, randlistIndex, check;
        private double volume = 0;
        private string[] files;
        private string sql;
        private bool count = false;
        SQLiteConnection dbConnection;
        SQLiteCommand command;
        SQLiteDataAdapter insert;
        SQLiteDataReader rdr;

        public PlayerWPF()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            DbConnect();
            ThemeLoader();
        }

        private void DbConnect()
        {
            if (!File.Exists("Player.sqlite"))
            {
                SQLiteConnection.CreateFile("Player.sqlite");
                dbConnection = new SQLiteConnection("Data Source=Player.sqlite;Version=3;");
                dbConnection.Open();
                sql = "Create Table PlayList (playList varchar(255))";
                command = new SQLiteCommand(sql, dbConnection);
                command.ExecuteNonQuery();
                sql = "Create Table Theme (theme int)";
                command = new SQLiteCommand(sql, dbConnection);
                command.ExecuteNonQuery();
            }
            else
            {
                dbConnection = new SQLiteConnection("Data Source=Player.sqlite;Version=3;");
                dbConnection.Open();
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            player.MediaOpened += new EventHandler(MusicOpen);
            player.MediaEnded += new EventHandler(MusicEnd);
        }

        private void MusicOpen(object sender, EventArgs e)
        {
            Peremotka.Maximum = Math.Round(player.NaturalDuration.TimeSpan.TotalSeconds);
        }

        private void MusicEnd(object sender, EventArgs e)
        {
            btNext_Click(sender, null);
        }

        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void btMin_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void TitleBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
            else
                DragMove();
        }

        private void ThemeLoader()
        {
            command = new SQLiteCommand("SELECT * FROM Theme", dbConnection);
            rdr = command.ExecuteReader();
            while (rdr.Read())
                check = Convert.ToInt32(rdr["theme"]);
            BackGround(check);
        }

        private void PlayListSaver_Click(object sender, RoutedEventArgs e)
        {
            command = new SQLiteCommand(@"DELETE FROM PlayList;", dbConnection);
            command.ExecuteNonQuery();
            for (int p = 0; p < audioList.Count; p++)
            {
                insert = new SQLiteDataAdapter();
                insert.InsertCommand = new SQLiteCommand("Insert Into PlayList Values (@playList)", dbConnection);
                insert.InsertCommand.Parameters.AddWithValue("@playList", audioList[p]);
                insert.InsertCommand.ExecuteNonQuery();
            }
        }

        private void PlayListLoader_Click(object sender, RoutedEventArgs e)
        {
            audioList.Clear();
            list.Items.Clear();
            infoBoxClear();
            command = new SQLiteCommand("SELECT * FROM PlayList", dbConnection);
            rdr = command.ExecuteReader();
            while (rdr.Read())
            {
                audioList.Add(rdr["playList"].ToString());
            }
            for (int i = 0; i < audioList.Count; i++)
                addMusic();
        }

        private void btReload_Click(object sender, RoutedEventArgs e)
        {
            infoBoxClear();
            image();
            TrackName.Text = "";
            audioList.Clear();
            i = 0;
            list.Items.Clear();
            player.Stop();
        }

        private void image()
        {
            ImageSource imageSource = new BitmapImage(new Uri("/Resources/music-album.png", UriKind.Relative));
            myImage.Source = imageSource;
        }

        private void btLoad_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog open = new System.Windows.Forms.OpenFileDialog();
            open.Multiselect = true;
            if (open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (String file in open.FileNames)
                {
                    audioList.Add(file);
                    pathList.Add(file);
                    addMusic();
                }
            }
        }

        private void addMusic()
        {
            try
            {
                TagLib.File file = TagLib.File.Create(audioList[i]);
                list.Items.Add(i + 1 + ". " + file.Tag.FirstPerformer + " - " + file.Tag.Title);
            }
            catch(ArgumentOutOfRangeException e) { }
            catch
            {
                var textInfo = new CultureInfo("ru-RU").TextInfo;
                CustomMessageBox.Show("Track not found: ", Path.GetFileNameWithoutExtension(textInfo.ToTitleCase(textInfo.ToLower(audioList[i]))));
            }
            finally
            {
                i++;
            }
        }

        private void list_Drop(object sender, DragEventArgs e)
        {
            files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string file1 in files)
            {
                audioList.Add(file1);
                pathList.Add(Path.GetFullPath(file1));
                addMusic();
            }
        }

        private void list_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                e.Effects = DragDropEffects.Copy;
            }
        }

        private void SoundOff_Click(object sender, RoutedEventArgs e)
        {
            mute();
            if (volume != 0)
            {
                volSlider.Value = volume;
            }
            else
            {
                volSlider.Value = 50;
            }
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(volSlider != null)
            {
                volumeChange();
            }
            if (volSlider.Value != 0 && SoundOff != null)
            {
                mute();
            }

            if (volSlider.Value == 0)
            {
                mute1();
            }
        }

        private void SoundOn_Click(object sender, RoutedEventArgs e)
        {
            volume = volSlider.Value;
            mute1();
            volSlider.Value = 0;
        }

        private void mute1()
        {
            player.IsMuted = true;
            SoundOn.Visibility = Visibility.Hidden;
            SoundOff.Visibility = Visibility.Visible;
        }

        private void mute()
        {
            player.IsMuted = false;
            SoundOff.Visibility = Visibility.Hidden;
            SoundOn.Visibility = Visibility.Visible;
        }

        private void btbefore_Click(object sender, RoutedEventArgs e)
        {
            infoBoxClear();
            player.Close();
            Peremotka.Value = 0;
            volumeChange();
            if (list.SelectedIndex > 0 && !count)
            {
                player.Open(new Uri(audioList[list.SelectedIndex - 1]));
                pathImage(list.SelectedIndex - 1);
                list.SelectedIndex = list.SelectedIndex - 1;
                informationStatus();
            }
            else if (count)
                random();
            else
                lastClick();
        }

        private void btPause_Click(object sender, RoutedEventArgs e)
        {
            player.Pause();
        }

        private void btPlay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty((string)TrackName.Text))
                {
                    infoBoxClear();
                    player.Close();
                    volumeChange();
                    player.Open(new Uri(audioList[list.SelectedIndex]));
                    pathImage(list.SelectedIndex);
                    timer();
                    informationStatus();
                }
                else
                {
                    player.Play();
                }
            }
            catch { }
        }

        private void List_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                infoBoxClear();
                player.Close();
                Peremotka.Value = 0;
                volumeChange();
                player.Open(new Uri(audioList[list.SelectedIndex]));
                pathImage(list.SelectedIndex);
                timer();
                informationStatus();
            }
            catch
            {
                player.Play();
            }
            
        }

        private void timer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 1);
            timer.Tick += new EventHandler(timerTick);
            timer.Start();
        }

        private void btNext_Click(object sender, RoutedEventArgs e)
        {
            infoBoxClear();
            player.Close();
            Peremotka.Value = 0;
            volumeChange();
            if (list.SelectedIndex < list.Items.Count - 1 && !count)
            {
                player.Open(new Uri(audioList[list.SelectedIndex + 1]));
                pathImage(list.SelectedIndex + 1);
                list.SelectedIndex = list.SelectedIndex + 1;
                informationStatus();
            }
            else if (count)
                random();
            else
                lastClick();
        }

        private void lastClick()
        {
            list.SelectedIndex = list.SelectedIndex;
            player.Open(new Uri(audioList[list.SelectedIndex]));
            pathImage(list.SelectedIndex);
            list.Focus();
            content();
            player.Play();
        }

        private void informationStatus()
        {
            labelTick();
            player.Play();
            list.Focus();
            content();
        }

        private void volumeChange()
        {
            player.Volume = Convert.ToDouble(volSlider.Value) / 170;
        }

        private void Peremotka_LostMouseCapture(object sender, MouseEventArgs e)
        {
            player.Pause();
            player.Position = TimeSpan.FromSeconds(Peremotka.Value);
            player.Play();
        }

        private void timerTick(object sender, EventArgs e)
        {
            if (player.NaturalDuration.HasTimeSpan)
            {
                Peremotka.Value = Math.Round(player.Position.TotalSeconds);
                labelTick();
            }
        }

        private void labelTick()
        {
            timerLabel.Content = TimeSpan.FromSeconds(Math.Round(Peremotka.Value)) + "/" + TimeSpan.FromSeconds(Peremotka.Maximum);
        }

        private void content()
        {
            TrackName.Text = list.SelectedItem.ToString();
        }

        private void pathImage(int p)
        {
            try
            {
                TagLib.File file = TagLib.File.Create(audioList[p]);
                infoBox.Items.Add("Title: " + file.Tag.Title);
                infoBox.Items.Add("Performer: " + file.Tag.FirstPerformer);
                infoBox.Items.Add("Album: " + file.Tag.Album);
                infoBox.Items.Add("Year: " + Convert.ToString(file.Tag.Year));
                TagLib.IPicture pic = file.Tag.Pictures[0];
                MemoryStream stream = new MemoryStream(pic.Data.Data);
                Image im = new Image();
                BitmapFrame bmp = BitmapFrame.Create(stream);
                myImage.Source = bmp;
                file.Save();
            }
            catch
            {
                image();
            }
        }

        private void Scatter_Click(object sender, RoutedEventArgs e)
        {
            if (!count)
            {
                count = true;
                Button myImg = (Button)sender;
                DropShadowEffect myEffect = new DropShadowEffect();
                myEffect.Color = Colors.Purple;
                myEffect.BlurRadius = 10;
                myEffect.ShadowDepth = 1;
                myEffect.Opacity = 1;
                myImg.Effect = myEffect;
            }
            else
            {
                count = false;
                Button myImg = (Button)sender;
                myImg.ClearValue(EffectProperty);
            }
        }

        private void random()
        {
            randlistIndex = randomSound();
            player.Open(new Uri(audioList[randlistIndex]));
            pathImage(randlistIndex);
            list.SelectedIndex = randlistIndex;
            informationStatus();
        }

        private int randomSound()
        {
            try
            {
                Random rnd = new Random();
                return randomNumb = rnd.Next(list.Items.Count);
            }
            catch
            {
                return randomSound();
            }
        }

        private void playImg_MouseEnter(object sender, MouseEventArgs e)
        {
            Image myImg = (Image)sender;
            DropShadowEffect myEffect = new DropShadowEffect();
            if (check == 0)
                myEffect.Color = Colors.White;
            else
                myEffect.Color = Colors.Black;
            myEffect.BlurRadius = 10;
            myEffect.ShadowDepth = 0;
            myEffect.Opacity = 1;
            myImg.Effect = myEffect;
            Cursor = Cursors.Hand;
        }

        private void playImg_MouseLeave(object sender, MouseEventArgs e)
        {
            Image myImg = (Image)sender;
            myImg.ClearValue(EffectProperty);
            Cursor = Cursors.Arrow;
        }

        private void all_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void all_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
        }

        private void Instagram_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.instagram.com/logan.pasha/");
        }

        private void infoBoxClear()
        {
            infoBox.Items.Clear();
        }

        private void BrightTheme_Click(object sender, RoutedEventArgs e)
        {
            check = 1;
            BackGround(check);
        }

        private void DefaultTheme_Click(object sender, RoutedEventArgs e)
        {
            check = 0;
            BackGround(check);
        }

        private void BackGround(int check)
        {
            LinearGradientBrush gradient = new LinearGradientBrush();
            LinearGradientBrush gradientTitle = new LinearGradientBrush();
            if (check == 1)
            {
                dockFirst.Background = Brushes.LightPink;
                dockSecond.Background = Brushes.LightPink;
                dockThird.Background = Brushes.LightPink;

                gradient.StartPoint = new Point(0.5, 0);
                gradient.EndPoint = new Point(0.5, 1);
                gradient.GradientStops.Add(new GradientStop(Colors.LightPink, 0.35));
                gradient.GradientStops.Add(new GradientStop(Colors.Aqua, 1));
                dockFourth.Background = gradient;

                timerLabel.Foreground = Brushes.Black;
                TrackName.Foreground = Brushes.Black;
                list.Foreground = Brushes.Black;
                infoBox.Foreground = Brushes.Black;

                gradientTitle.StartPoint = new Point(0.5, 0);
                gradientTitle.EndPoint = new Point(0.5, 1);
                gradientTitle.GradientStops.Add(new GradientStop(Colors.LightPink, 1));
                gradientTitle.GradientStops.Add(new GradientStop(Colors.Aqua, 0.35));
                TitleBox.Background = gradientTitle;
                Caption.Foreground = Brushes.Black;
            }
            else
            {
                var bc = new BrushConverter();
                dockFirst.Background = (Brush)bc.ConvertFrom("#FF2E46B2");
                dockSecond.Background = (Brush)bc.ConvertFrom("#FF2E46B2");
                dockThird.Background = (Brush)bc.ConvertFrom("#FF2E46B2");

                gradient.StartPoint = new Point(0.5, 0);
                gradient.EndPoint = new Point(0.5, 1);
                gradient.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF2E46B2"), 0.35));
                gradient.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF632470"), 1));
                dockFourth.Background = gradient;

                timerLabel.Foreground = Brushes.White;
                TrackName.Foreground = Brushes.White;
                list.Foreground = Brushes.White;
                infoBox.Foreground = Brushes.White;

                gradientTitle.StartPoint = new Point(0.5, 0);
                gradientTitle.EndPoint = new Point(0.5, 1);
                gradientTitle.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF2E46B2"), 1));
                gradientTitle.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF632470"), 0.35));
                TitleBox.Background = gradientTitle;
                Caption.Foreground = Brushes.White;
            }
            ThemeSaver(check);
        }

        private void ThemeSaver(int check)
        {
            command = new SQLiteCommand(@"DELETE FROM Theme;", dbConnection);
            command.ExecuteNonQuery();
            insert = new SQLiteDataAdapter();
            insert.InsertCommand = new SQLiteCommand("Insert Into Theme Values (@theme)", dbConnection);
            insert.InsertCommand.Parameters.AddWithValue("@theme", check);
            insert.InsertCommand.ExecuteNonQuery();
        }
    }
}