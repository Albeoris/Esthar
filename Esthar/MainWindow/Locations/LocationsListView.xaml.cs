using System;
using System.Collections;
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
using Esthar.Data.Transform;
using Esthar.UI;

namespace Esthar
{
    /// <summary>
    /// Логика взаимодействия для MainTreeView.xaml
    /// </summary>
    public partial class LocationsListView
    {
        public LocationsListControl ParentListControl { get; set; }

        public LocationsListView()
        {
            InitializeComponent();
            
            SizeChanged += OnSizeChanged;
            OnSizeChanged(this, null);
        }

        void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            TitleColumn.Width = Width - NameColumn.Width;
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            foreach (LocationView oldItems in e.RemovedItems)
                oldItems.Location.Dispose();

            foreach (LocationView newItem in e.AddedItems)
                ParentListControl.ParentTab.LocationControl.DataContext = newItem.Location;
        }

        private void OnTagsMenuClick(object sender, RoutedEventArgs e)
        {
            IList selectedItems = SelectedItems;
            TagsEditWindow wnd = new TagsEditWindow(Options.UserTags.GetLocationTags(), selectedItems.Cast<IUserTagsHandler>());
            if (wnd.ShowDialog() == true)
                foreach (LocationView view in selectedItems)
                    view.Location.SaveRequest |= LocationProperty.Tags;
        }
    }
}
