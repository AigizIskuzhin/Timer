using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Timer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly struct TimerStruct
        {
            public TimerStruct(int hours, int minutes, int seconds, string title = "")
            {
                Hours = hours;
                Minutes = minutes;
                Seconds = seconds;
                Title = title;
            }
            public string Title { get; }
            public int Hours { get; }
            public int Minutes { get; }
            public int Seconds { get; }
            public string AsText
            {
                get
                {
                    string text = "";
                    text = Hours == 0 ? "00:" : Hours < 10 ? "0" + Hours + ":" : Hours + ":";
                    text += Minutes == 0 ? "00:" : Minutes < 10 ? "0" + Minutes + ":" : Minutes + ":";
                    text += Seconds == 0 ? "00" : Seconds < 10 ? "0" + Seconds: Seconds;
                    return text;
                }
            }
        }
        private readonly DispatcherTimer TimerSelf = new();
        private readonly OpenFileDialog OpenFileDialog = new();
        private readonly MediaPlayer MediaPlayer = new();
        public MainWindow()
        {
            InitializeComponent();
            List.SelectionChanged += List_SelectionChanged;
            TimerSelf.Tick += TimerSelf_Tick;
            TimerSelf.Interval = new TimeSpan(0, 0, 0, 1);
        }

        private int seconds = 0;
        private void TimerSelf_Tick(object sender, EventArgs e)
        {
            seconds -= 1;
            if (seconds < 0)
            {
                TimerSelf.Stop();
                
                Dispatcher.Invoke(() =>
                {
                    MediaPlayer.Play();
                });
                return;
            }
            TimerOne.Dispatcher.Invoke(() =>
            {
                TimerOne.Text = $"{seconds / 3600:00}:{seconds / 60 % 60:00}:{seconds % 60:00}";
            });
        }

        public void AddTimerToList(TimerStruct timer)
        {
            List.Items.Add(timer);
        }
        private void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = List.SelectedItem;
            var isNull = item == null;
            if (!isNull)
            {
                CurrentTimer = (TimerStruct)List.SelectedItem;
                Title.Text = CurrentTimer.Title;
                TimerOne.Text = CurrentTimer.AsText;

                RemoveBtn.Visibility = Visibility.Visible;
                StartBtn.Visibility = Visibility.Visible;
                PauseBtn.Visibility = Visibility.Visible;
                DropBtn.Visibility = Visibility.Visible;
                NextBtn.Visibility = Visibility.Visible;
                MusicBtn.Visibility = Visibility.Visible;
            }
            else
            {
                Title.Text = "";
                TimerOne.Text = "";
                
                RemoveBtn.Visibility = Visibility.Collapsed;
                StartBtn.Visibility = Visibility.Collapsed;
                PauseBtn.Visibility = Visibility.Collapsed;
                DropBtn.Visibility = Visibility.Collapsed;
                NextBtn.Visibility = Visibility.Collapsed;
                MusicBtn.Visibility = Visibility.Collapsed;
            }

        }

        private TimerStruct _CurrentStruct;

        private TimerStruct CurrentTimer
        {
            get => _CurrentStruct;
            set
            {
                seconds = (value.Hours * 60 * 60) + value.Minutes * 60 + value.Seconds;
                
                Title.Text = CurrentTimer.Title;
                TimerOne.Text = CurrentTimer.AsText;
                _CurrentStruct = value;
            }
        }

        private void OnBackgroundClick(object sender, MouseButtonEventArgs e) => Keyboard.ClearFocus();
        private double CalculateSize(double size, int delta)
        {
            size += delta > 0 ? +1 : -1;
            size = size < 14 ? 14 : size;
            return size;
        }
        private void OnMouseWheelResize(object sender, MouseWheelEventArgs e)
        {
            switch (sender)
            {
                case TextBlock textBlock:
                    textBlock.FontSize = CalculateSize(textBlock.FontSize, e.Delta);
                    break;
                case TextBox textBox:
                    textBox.FontSize = CalculateSize(textBox.FontSize, e.Delta);;
                    break;
            }
        }
        private void OnClickAddTimer(object sender, RoutedEventArgs e) => new AddTimer(this).ShowDialog();

        private void OnClickRemoveSelected(object sender, RoutedEventArgs e)
        {
            var item = List.SelectedItem;
            if (item != null)
            {
                if (item is TimerStruct t)
                {
                    var c = CurrentTimer;
                    if (t.Equals(c)) TimerSelf.Stop();
                }
                List.Items.RemoveAt(List.Items.IndexOf(item));
            }
        }

        private void OnClickStartTimer(object sender, RoutedEventArgs e) => TimerSelf.Start();

        private void OnClickPauseTimer(object sender, RoutedEventArgs e) => TimerSelf.Stop();

        private void OnClickDropTimer(object sender, RoutedEventArgs e)
        {
            MediaPlayer.Stop();
            TimerSelf.Stop();
            CurrentTimer = CurrentTimer;
        }

        private void OnClickSetupNext(object sender, RoutedEventArgs e)
        {
            int current = List.SelectedIndex;
            if (List.Items.Count > current + 1)
            {
                TimerSelf.Stop();
                MediaPlayer.Stop();
                List.SelectedIndex = current + 1;
            }
            else
            {
                if(List.Items.Count==1)return;
                TimerSelf.Stop();
                MediaPlayer.Stop();
                List.SelectedIndex = 0;
            }
        }

        private void OnClickSelectMusic(object sender, RoutedEventArgs e)
        {
            if (OpenFileDialog.ShowDialog(this).Value)
            {
                FileInfo file = new(OpenFileDialog.FileName);
                MediaPlayer.Open(new Uri(file.FullName, UriKind.Absolute));
                ((Button)sender).Content = file.Name.Length > 10 ? file.Name.Substring(0, 10) + "..." : file.Name;
            }
        }
    }
}
