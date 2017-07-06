using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Boom2maru
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool isClosed = false;
        private System.Diagnostics.Process[] processes = new System.Diagnostics.Process[] { };

        public DispatcherTimer timer = null;
        TimeSpan _interval = new TimeSpan(0, 0, 5);
        TimeSpan Interval
        {
            get
            {
                return _interval;
            }
            set
            {
                if (value != _interval)
                {
                    _interval = value;
                    nowIntervalTextBlock.Text = NowIntervalText;
                }
            }
        }
        System.Diagnostics.Process targetProcess = null;

        public string NowIntervalText
        {
            get
            {
                return $"{Interval.Minutes}分{Interval.Seconds}秒";
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            ReloadProcesses();
            nowIntervalTextBlock.Text = NowIntervalText;
        }

        public MainWindow(DispatcherTimer timer)
        {
            InitializeComponent();
            ReloadProcesses();
            this.timer = timer;
            this.Interval = timer.Interval;
            startButton.IsEnabled = false;
            stopButton.IsEnabled = true;
            nowIntervalTextBlock.Text = NowIntervalText;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            isClosed = true;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ReloadProcesses()
        {
            processes = System.Diagnostics.Process.GetProcesses().Where(p => !string.IsNullOrEmpty(p.MainWindowTitle)).ToArray();

            processesCombobox.Items.Clear();
            foreach (var p in processes)
            {
                processesCombobox.Items.Add(p.MainWindowTitle + " - " + p.ProcessName);
            }
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            ReloadProcesses();
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            if (processesCombobox.SelectedIndex == -1)
            {
                MessageBox.Show("撮影するウィンドウ名を指定してください");
                return;
            }

            targetProcess = processes[processesCombobox.SelectedIndex];
            timer?.Stop();
            timer = new DispatcherTimer()
            {
                Interval = Interval
            };
            timer.Tick += (sender2, e2) =>
            {
                if (targetProcess == null || targetProcess.HasExited)
                {
                    timer.Stop();
                    return;
                }
                // 保存
                var image = WindowCapture.GetBitmapImage(targetProcess.MainWindowHandle);
                if (image == null) return;
                using (var fs = new FileStream($"./save/{GetNextFileNumber()}.png", FileMode.Create))
                {
                    var enc = new PngBitmapEncoder();
                    enc.Frames.Add(BitmapFrame.Create(image));
                    enc.Save(fs);
                    fs.Close();
                }
            };
            timer.Start();
            startButton.IsEnabled = false;
            stopButton.IsEnabled = true;
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            timer?.Stop();
            startButton.IsEnabled = true;
            stopButton.IsEnabled = false;
        }

        private void IntervalDecideButton_Click(object sender, RoutedEventArgs e)
        {
            if (!timePicker.isCorrect)
            {
                MessageBox.Show("時刻は整数で1から60の範囲で指定してください");
                return;
            }
            Interval = timePicker.GetTimeSpan();
        }

        private static int GetNextFileNumber()
        {
            int n = 0;
            var dir = new DirectoryInfo("./save/");
            foreach (var f in dir.GetFiles())
            {
                var m = Regex.Match(f.Name, @"(\d+)\..*");
                if (m.Success)
                {
                    int i = int.Parse(m.Groups[1].Value);
                    if (i > n) n = i;
                }
            }
            return n + 1;
        }
    }
}
