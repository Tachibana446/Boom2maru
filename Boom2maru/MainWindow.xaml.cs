using Microsoft.WindowsAPICodePack.Dialogs;
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

        private static DirectoryInfo SaveDirectory = new DirectoryInfo("./ScreenShots");
        private static int FileFormatSelectedIndex = 0;

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

        public MainWindow(DispatcherTimer timer = null)
        {
            Init();
            if (timer != null)
            {
                this.timer = timer;
                this.Interval = timer.Interval;
                startButton.IsEnabled = false;
                stopButton.IsEnabled = true;
                nowIntervalTextBlock.Text = NowIntervalText;
            }
        }

        /// <summary>
        /// コンストラクタで共通する処理
        /// </summary>
        private void Init()
        {
            InitializeComponent();
            ReloadProcesses();
            nowIntervalTextBlock.Text = NowIntervalText;
            saveFolderTextBox.Text = SaveDirectory.FullName;
            fileFormatCombobox.SelectedIndex = FileFormatSelectedIndex;
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
                if (!SaveDirectory.Exists) SaveDirectory.Create();
                var image = WindowCapture.GetBitmapImage(targetProcess.MainWindowHandle);
                if (image == null) return;
                var extensions = new string[] { "png", "jpg" };
                using (var fs = new FileStream($"{SaveDirectory.FullName}/{GetNextFileNumber()}.{extensions[fileFormatCombobox.SelectedIndex]}",
                    FileMode.Create))
                {
                    BitmapEncoder enc;
                    switch (fileFormatCombobox.SelectedIndex)
                    {
                        case 0: enc = new PngBitmapEncoder(); break;
                        default: enc = new JpegBitmapEncoder(); break;
                    }
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
            if (!SaveDirectory.Exists) return 1;
            foreach (var f in SaveDirectory.GetFiles())
            {
                var m = Regex.Match(f.Name, @"(\d+)\.(png|jpg)");
                if (m.Success)
                {
                    int i = int.Parse(m.Groups[1].Value);
                    if (i > n) n = i;
                }
            }
            return n + 1;
        }

        private void SaveFolderSelectButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SaveDirectory = new DirectoryInfo(dialog.FileName);
                saveFolderTextBox.Text = SaveDirectory.FullName;
            }
        }

        /// <summary>
        /// ファイルフォーマットを変更した時
        /// </summary>
        private void fileFormatCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FileFormatSelectedIndex = fileFormatCombobox.SelectedIndex;
        }
    }
}
