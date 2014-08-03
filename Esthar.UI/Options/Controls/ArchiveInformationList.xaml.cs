using System.Collections.ObjectModel;
using System.Windows;

namespace Esthar.UI
{
    /// <summary>
    /// Логика взаимодействия для ArchivesStateList.xaml
    /// </summary>
    public partial class ArchiveInformationList
    {
        public ArchiveInformationList()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ArchivesProperty = DependencyProperty.Register("Archives", typeof(ObservableCollection<ArchiveInformationView>), typeof(ArchiveInformationList), new PropertyMetadata(new ObservableCollection<ArchiveInformationView>()));

        public ObservableCollection<ArchiveInformationView> Archives
        {
            get { return (ObservableCollection<ArchiveInformationView>)GetValue(ArchivesProperty); }
            set { SetValue(ArchivesProperty, value); }
        }
    }
}
