using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
using Esthar.Core;
using Esthar.Data.Transform;
using Xceed.Wpf.Toolkit;

namespace Esthar
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            Closing += OnClosing;
        }

        void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainTabs.SafeDispose();
        }
    }
}
