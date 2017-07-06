using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace Boom2maru
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool isClosed = false;
        private System.Diagnostics.Process[] processes = new System.Diagnostics.Process[] { };

        public MainWindow()
        {
            InitializeComponent();
            ReloadProcesses();
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
                processesCombobox.Items.Add(p.ProcessName);
            }
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            ReloadProcesses();
        }
    }
}
