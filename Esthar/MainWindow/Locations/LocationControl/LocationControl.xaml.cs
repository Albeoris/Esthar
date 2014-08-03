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
using Esthar.Core;

namespace Esthar
{
    /// <summary>
    /// Логика взаимодействия для LocationControl.xaml
    /// </summary>
    public partial class LocationControl : IDisposable
    {
        public LocationControl()
        {
            InitializeComponent();
        }

        public void Dispose()
        {
            TextTab.SafeDispose();
            //ScriptsTab.SafeDispose();
        }
    }
}
