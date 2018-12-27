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
        private int i = 0, p = 0, randomNumb, randlistIndex;
        private double volume = 0, locX, locY;
        private string[] files;
        private string str, sql;
        private bool count = false, backGround = false;
        Info aboutSong = new Info();
        SQLiteConnection dbConnection;
        SQLiteCommand command;
        SQLiteDataAdapter insert;
        SQLiteDataReader rdr;

        public PlayerWPF()
        {
            InitializeComponent();
            dbConnectAsync();
        }

        private async void dbConnectAsync()
        {
            await Task.Run(() => dbConnect());
        }

        private void dbConnect()
        {
            if (!File.Exists("PlayList.sqlite"))
            {
                SQLiteConnection.CreateFile("PlayList.sqlite");
                dbConnection = new SQLiteConnection("Data Source=PlayList.sqlite;Version=3;");
                dbConnection.Open();
                sql = "Create Table PlayList (playList varchar(255))";
                command = new SQLiteCommand(sql, dbConnection);
                command.ExecuteNonQuery();
            }
            else
            {
                dbConnection = new SQLiteConnection("Data Source=PlayList.sqlite;Version=3;");
                dbConnection.Open();
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            locationWindows();
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
            aboutSong.Close();
        }

        private void btMin_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
            aboutSong.Visibility = Visibility.Hidden;
        }

        private void TitleBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && aboutSong.Visibility == Visibility.Visible)
            {
                aboutSong.Visibility = Visibility.Hidden;
                DragMove();
                locationWindows();
                aboutSongLocation();
                aboutSong.Visibility = Visibility.Visible;
            }
            else
            {
                DragMove();
                locationWindows();
            }
        }

        private void info_Click(object sender, RoutedEventArgs e)
        {
            if (aboutSong.Visibility == Visibility.Hidden)
            {
                aboutSongLocation();
                aboutSong.Visibility = Visibility.Visible;
            }
            else
                aboutSong.Visibility = Visibility.Hidden;
        }

        private void BtSettings_Click(object sender, RoutedEventArgs e)
        {
            if (!backGround)
            {
                backGround = true;
                customMainBackGround();
            }
            else
            {
                backGround = false;
                defaulBackGround();
            }
        }

        private void defaulBackGround()
        {
            mainFormDefaultBackGround();
            defaulTitleBoxGradient();
            infoDefaultBackGround();
        }

        private void mainFormDefaultBackGround()
        {
            var bc = new BrushConverter();
            dockFirst.Background = (Brush)bc.ConvertFrom("#FF2E46B2");
            dockSecond.Background = (Brush)bc.ConvertFrom("#FF2E46B2");
            dockThird.Background = (Brush)bc.ConvertFrom("#FF2E46B2");

            LinearGradientBrush gradient = new LinearGradientBrush();
            gradient.StartPoint = new Point(0.5, 0);
            gradient.EndPoint = new Point(0.5, 1);
            gradient.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF2E46B2"), 0.35));
            gradient.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF632470"), 1));
            dockFourth.Background = gradient;

            label.Foreground = Brushes.White;
            Pesnya.Foreground = Brushes.White;
            list.Foreground = Brushes.White;
        }

        private void infoDefaultBackGround()
        {
            var bc = new BrushConverter();
            aboutSong.infoPanel.Background = (Brush)bc.ConvertFrom("#FF2E46B2");
            aboutSong.infoBox.Foreground = Brushes.White;
            aboutSong.infoText.Foreground = Brushes.White;
            LinearGradientBrush gradient = new LinearGradientBrush();
            gradient.StartPoint = new Point(0.5, 0);
            gradient.EndPoint = new Point(0.5, 1);
            gradient.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF2E46B2"), 1));
            gradient.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF632470"), 0.35));
            aboutSong.TitleBox.Background = gradient;
        }

        private void defaulTitleBoxGradient()
        {
            LinearGradientBrush gradient = new LinearGradientBrush();
            gradient.StartPoint = new Point(0.5, 0);
            gradient.EndPoint = new Point(0.5, 1);
            gradient.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF2E46B2"), 1));
            gradient.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF632470"), 0.35));
            TitleBox.Background = gradient;
            Caption.Foreground = Brushes.White;
        }

        private void customMainBackGround()
        {
            customMainTitleBoxGradient();
            mainFormBackGround();
            infoCustomBackGround();
        }

        private void mainFormBackGround()
        {
            dockFirst.Background = Brushes.LightPink;
            dockSecond.Background = Brushes.LightPink;
            dockThird.Background = Brushes.LightPink;

            LinearGradientBrush gradient = new LinearGradientBrush();
            gradient.StartPoint = new Point(0.5, 0);
            gradient.EndPoint = new Point(0.5, 1);
            gradient.GradientStops.Add(new GradientStop(Colors.LightPink, 0.35));
            gradient.GradientStops.Add(new GradientStop(Colors.Aqua, 1));
            dockFourth.Background = gradient;
            
            label.Foreground = Brushes.Black;
            Pesnya.Foreground = Brushes.Black;
            list.Foreground = Brushes.Black;
        }

        private void infoCustomBackGround()
        {
            LinearGradientBrush gradient = new LinearGradientBrush();
            gradient.StartPoint = new Point(0.5, 0);
            gradient.EndPoint = new Point(0.5, 1);
            gradient.GradientStops.Add(new GradientStop(Colors.LightPink, 1));
            gradient.GradientStops.Add(new GradientStop(Colors.Aqua, 0.35));
            aboutSong.TitleBox.Background = gradient;
            aboutSong.infoBox.Foreground = Brushes.Black;
            aboutSong.infoPanel.Background = Brushes.LightPink;
            aboutSong.infoText.Foreground = Brushes.Black;
        }

        private void customMainTitleBoxGradient()
        {
            LinearGradientBrush gradient = new LinearGradientBrush();
            gradient.StartPoint = new Point(0.5, 0);
            gradient.EndPoint = new Point(0.5, 1);
            gradient.GradientStops.Add(new GradientStop(Colors.LightPink, 1));
            gradient.GradientStops.Add(new GradientStop(Colors.Aqua, 0.35));
            TitleBox.Background = gradient;
            Caption.Foreground = Brushes.Black;
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
            image();
            Pesnya.Text = "";
            audioList.Clear();
            i = 0;
            list.Items.Clear();
            clear();
            player.Stop();
        }

        private void clear()
        {
            aboutSong.infoBox.Items.Clear();
            aboutSong.infoText.Text = "";
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
                slider.Value = volume;
            }
            else
            {
                slider.Value = 50;
            }
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(slider!= null)
            {
                volumeChange();
            }
            if (slider.Value != 0 && SoundOff != null)
            {
                mute();
            }

            if (slider.Value == 0)
            {
                mute1();
            }
        }

        private void SoundOn_Click(object sender, RoutedEventArgs e)
        {
            volume = slider.Value;
            mute1();
            slider.Value = 0;
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
            clear();
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
                scatter();
            else
                lastClick();
        }

        private void btPause_Click(object sender, RoutedEventArgs e)
        {
            player.Pause();
        }

        private void btPlay_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty((string)Pesnya.Text))
            {
                clear();
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

        private void List_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                clear();
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
            clear();
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
                scatter();
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
            player.Volume = Convert.ToDouble(slider.Value) / 170;
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
            label.Content = TimeSpan.FromSeconds(Math.Round(Peremotka.Value)) + "/" + TimeSpan.FromSeconds(Peremotka.Maximum);
        }

        private void content()
        {
            Pesnya.Text = list.SelectedItem.ToString();
            if (list.SelectedItem != null)
            {
                aboutSong.infoText.Text = list.SelectedItem.ToString();
            }
        }

        private void pathImage(int p)
        {
            try
            {
                TagLib.File file = TagLib.File.Create(audioList[p]);
                aboutSong.infoBox.Items.Add("Title: " + file.Tag.Title);
                aboutSong.infoBox.Items.Add("Performer: " + file.Tag.FirstPerformer);
                aboutSong.infoBox.Items.Add("Album: " + file.Tag.Album);
                aboutSong.infoBox.Items.Add("Year: " + Convert.ToString(file.Tag.Year));
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

        private void scatter()
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
            if (!backGround)
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

        private void aboutSongLocation()
        {
            aboutSong.Top = locX;
            aboutSong.Left = locY - 284;
        }

        private void locationWindows()
        {
            locX = this.Top;
            locY = this.Left;
        }

        private void Instagram_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.instagram.com/logan.pasha/");
        }
    }
}