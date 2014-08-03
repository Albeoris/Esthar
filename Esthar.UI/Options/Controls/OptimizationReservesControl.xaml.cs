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

namespace Esthar.UI
{
    /// <summary>
    /// Логика взаимодействия для ArchvieReservesControl.xaml
    /// </summary>
    public partial class OptimizationReservesControl
    {
        public OptimizationReservesControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty AbsoluteValueProperty = DependencyProperty.Register("AbsoluteValue", typeof(uint), typeof(OptimizationReservesControl), new PropertyMetadata(default(uint)));
        public static readonly DependencyProperty RelativeValueProperty = DependencyProperty.Register("RelativeValue", typeof(uint), typeof(OptimizationReservesControl), new PropertyMetadata(default(uint)));

        public uint AbsoluteValue
        {
            get { return (uint)GetValue(AbsoluteValueProperty); }
            set { SetValue(AbsoluteValueProperty, value); }
        }

        public uint RelativeValue
        {
            get { return (uint)GetValue(RelativeValueProperty); }
            set { SetValue(RelativeValueProperty, value); }
        }
    }
}
