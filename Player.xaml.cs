﻿using System;
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
using Player;

namespace MusicPlayer
{
    public partial class Player : Window
    {
        public List<String> audioList = new List<String>();
        public List<String> pathList = new List<String>();
        private MediaPlayer player = new MediaPlayer();
        public int i = 0, p = 0, randomNumb, randlistIndex;
        public double volume = 0, locX, locY;
        public string[] files;
        public string str, sql;
        public bool count = false;
        Info form2 = new Info();
        SQLiteConnection dbConnection;
        SQLiteCommand command;
        SQLiteDataAdapter insert;
        SQLiteDataReader rdr;

        public Player()
        {
            InitializeComponent();
            dbConnect();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            locationWindows();
            player.MediaOpened += new EventHandler(MusicOpen);
            player.MediaEnded += new EventHandler(MusicEnd);
        }

        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
            form2.Close();
        }

        private void btMin_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
            form2.Visibility = Visibility.Hidden;
        }

        private void TitleBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && form2.Visibility == Visibility.Visible)
            {
                form2.Visibility = Visibility.Hidden;
                DragMove();
                locationWindows();
                form2Location();
                form2.Visibility = Visibility.Visible;
            }
            else
            {
                DragMove();
                locationWindows();
            }
        }

        private void info_Click(object sender, RoutedEventArgs e)
        {
            if (form2.Visibility == Visibility.Hidden)
            {
                form2Location();
                form2.Visibility = Visibility.Visible;
            }
            else
                form2.Visibility = Visibility.Hidden;
        }

        private void PlayListSaver_Click(object sender, RoutedEventArgs e)
        {
            try
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
            catch (SQLiteException ew)
            {
                MessageBox.Show("SQLite Error:" + ew.ToString());
            }
        }

        private void PlayListLoader_Click(object sender, RoutedEventArgs e)
        {
            audioList.Clear();
            list.Items.Clear();
            list2.Items.Clear();
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
            Pesnya.Content = "";
            audioList.Clear();
            i = 0;
            list.Items.Clear();
            list2.Items.Clear();
            clear();
            player.Stop();
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

        private void Scatter_Click(object sender, RoutedEventArgs e)
        {
            if (!count)
            {
                count = true;
                Button myImg = (Button)sender;
                DropShadowEffect myEffect = new DropShadowEffect();
                myEffect.Color = Colors.Lavender;
                myEffect.BlurRadius = 10;
                myEffect.ShadowDepth = 0;
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
            if (String.IsNullOrEmpty((string)Pesnya.Content))
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
            clear();
            player.Close();
            Peremotka.Value = 0;
            volumeChange();
            player.Open(new Uri(audioList[list.SelectedIndex]));
            pathImage(list.SelectedIndex);
            timer();
            informationStatus();
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

        private void Peremotka_LostMouseCapture(object sender, MouseEventArgs e)
        {
            player.Pause();
            player.Position = TimeSpan.FromSeconds(Peremotka.Value);
            player.Play();
        }

        private void labelTick()
        {
            label.Content = TimeSpan.FromSeconds(Math.Round(Peremotka.Value)) + "/" + TimeSpan.FromSeconds(Peremotka.Maximum);
        }

        private void timer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 1);
            timer.Tick += new EventHandler(timerTick);
            timer.Start();
        }

        private void timerTick(object sender, EventArgs e)
        {
            if (player.NaturalDuration.HasTimeSpan)
            {
                Peremotka.Value = Math.Round(player.Position.TotalSeconds);
                labelTick();
            }
        }

        private void MusicOpen(object sender, EventArgs e)
        {
            Peremotka.Maximum = Math.Round(player.NaturalDuration.TimeSpan.TotalSeconds);
        }

        private void MusicEnd(object sender, EventArgs e)
        {
            btNext_Click(sender, null);
        }

        private void scatter()
        {
            randlistIndex = randomSound();
            player.Open(new Uri(audioList[randlistIndex]));
            pathImage(randlistIndex);
            list.SelectedIndex = randlistIndex;
            informationStatus();
        }

        private void informationStatus()
        {
            labelTick();
            player.Play();
            list.Focus();
            content();
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

        private void volumeChange()
        {
            player.Volume = Convert.ToDouble(slider.Value) / 170;
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

        private void content()
        {
            Pesnya.Content = list.SelectedItem;
            if (list.SelectedItem != null)
            {
                form2.infoText.Text = list.SelectedItem.ToString();
            }
        }

        private void pathImage(int p)
        {
            try
            {
                TagLib.File file = TagLib.File.Create(audioList[p]);
                form2.infoBox.Items.Add("Title: " + file.Tag.Title);
                form2.infoBox.Items.Add("Performer: " + file.Tag.FirstPerformer);
                form2.infoBox.Items.Add("Album: " + file.Tag.Album);
                form2.infoBox.Items.Add("Year: " + Convert.ToString(file.Tag.Year));
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

        private void image()
        {
            ImageSource imageSource = new BitmapImage(new Uri("/Resources/music-album.png", UriKind.Relative));
            myImage.Source = imageSource;
        }

        private void addMusic()
        {
            try
            {
                var textInfo = new CultureInfo("ru-RU").TextInfo;
                TagLib.File file = TagLib.File.Create(audioList[i]);
                string trackName = Path.GetFileNameWithoutExtension(textInfo.ToTitleCase(textInfo.ToLower(audioList[i])));
                list.Items.Add(i + 1 + ". " + trackName);
                list2.Items.Add(file.Properties.Duration.ToString("mm\\:ss"));
                i++;
            }
            catch
            {
                MessageBox.Show("Track not found", "Error", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
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

        private void playImg_MouseEnter(object sender, MouseEventArgs e)
        {
            Image myImg = (Image)sender;
            DropShadowEffect myEffect = new DropShadowEffect();
            myEffect.Color = Colors.White;
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

        private void clear()
        {
            form2.infoBox.Items.Clear();
            form2.infoText.Text = "";
        }

        private void all_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void all_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
        }

        private void scroll_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            sv1.ScrollToVerticalOffset(sv1.VerticalOffset - (double)e.Delta);
            sv2.ScrollToVerticalOffset(sv2.VerticalOffset - (double)e.Delta);
        }

        private void ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (sender == sv1)
            {
                sv2.ScrollToVerticalOffset(e.VerticalOffset);
            }
            else
            {
                sv1.ScrollToVerticalOffset(e.VerticalOffset);
            }
        }

        private void locationWindows()
        {
            locX = this.Top;
            locY = this.Left;
        }

        private void form2Location()
        {
            form2.Top = locX;
            form2.Left = locY - 284;
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
    }
}