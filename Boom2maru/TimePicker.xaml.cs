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
    /// TimePicker.xaml の相互作用ロジック
    /// </summary>
    public partial class TimePicker : UserControl
    {
        /// <summary>
        /// 時間が整数で、かつ1~60の範囲内か
        /// </summary>
        public bool isCorrect
        {
            get
            {
                try
                {
                    var n = int.Parse(textBox.Text);
                    if (n <= 0 || n > 60) return false;
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public int NowCount
        {
            get
            {
                try
                {
                    int n = int.Parse(textBox.Text);
                    return n;
                }
                catch (Exception)
                {
                    return 1;
                }
            }
            set
            {
                textBox.Text = value.ToString();
            }
        }

        public TimePicker()
        {
            InitializeComponent();
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void upButtonClick(object sender, RoutedEventArgs e)
        {
            textBox.Text = (NowCount + 1).ToString();
        }

        private void downButtonClick(object sender, RoutedEventArgs e)
        {
            textBox.Text = (NowCount - 1).ToString();
        }
    }
}
