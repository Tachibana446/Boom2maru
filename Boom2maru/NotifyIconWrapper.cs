using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Boom2maru
{
    public partial class NotifyIconWrapper : Component
    {
        private MainWindow window = null;

        public NotifyIconWrapper()
        {
            InitializeComponent();

            // コンテキストメニューのイベント
            OpenToolStripMenuItem1.Click += OpenToolStripMenuItem1_Click;
            ExitToolStripMenuItem1.Click += ExitToolStripMenuItem1_Click;
        }

        private void ExitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OpenToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (window == null || window.isClosed)
            {
                window = new MainWindow();
            }
            window.Show();
        }

        public NotifyIconWrapper(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
