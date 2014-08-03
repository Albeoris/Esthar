using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Esthar.Core;
using Esthar.Data;
using Esthar.Data.Transform;
using Esthar.UI;

namespace Esthar
{
    /// <summary>
    /// Логика взаимодействия для LocationSaveDialog.xaml
    /// </summary>
    public partial class LocationSaveDialog : Window
    {
        public static void ShowSave(IEnumerable<Location> locations)
        {
            LocationSaveDialog dlg = new LocationSaveDialog(Mode.Save, locations);
            dlg.ShowDialog();
        }

        public static void ShowImport(IEnumerable<Location> locations)
        {
            LocationSaveDialog dlg = new LocationSaveDialog(Mode.Import, locations);
            dlg.ShowDialog();
        }

        private readonly Mode _mode;

        private LocationSaveDialog(Mode mode, IEnumerable<Location> locations)
        {
            InitializeComponent();

            _mode = mode;
            LocationsListBox.ItemsSource = locations.Select(l => new LocationView(l));
        }

        private void OnListSelectAll(object sender, RoutedEventArgs e)
        {
            try
            {
                ListBox list = (ListBox)sender;
                IEnumerable<LocationView> items = (IEnumerable<LocationView>)list.ItemsSource;
                foreach (LocationView item in items)
                    item.IsSelected = true;
            }
            catch (Exception ex)
            {
                UIHelper.ShowError(ex);
            }
        }

        private void OnListClearSelection(object sender, RoutedEventArgs e)
        {
            try
            {
                ListBox list = (ListBox)sender;
                IEnumerable<LocationView> items = (IEnumerable<LocationView>)list.ItemsSource;
                foreach (LocationView item in items)
                    item.IsSelected = false;
            }
            catch (Exception ex)
            {
                UIHelper.ShowError(ex);
            }
        }

        private void OnListInvertSelection(object sender, RoutedEventArgs e)
        {
            try
            {
                ListBox list = (ListBox)sender;
                IEnumerable<LocationView> items = (IEnumerable<LocationView>)list.ItemsSource;
                foreach (LocationView item in items)
                    item.IsSelected = !item.IsSelected;
            }
            catch (Exception ex)
            {
                UIHelper.ShowError(ex);
            }
        }

        private void OnOkButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                IEnumerable<LocationView> items = ((IEnumerable<LocationView>)LocationsListBox.ItemsSource).Where(i => i.IsSelected);
                foreach (LocationView view in items)
                    switch (_mode)
                    {
                        case Mode.Save:
                            view.Location.SaveRequestedData();
                            break;
                        case Mode.Import:
                            view.Location.ImportRequestedData();
                            break;
                    }

                Archives.GetInfo(ArchiveName.Field).Update();

                DialogResult = true;
            }
            catch (Exception ex)
            {
                UIHelper.ShowError(ex);
            }
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogResult = false;
            }
            catch (Exception ex)
            {
                UIHelper.ShowError(ex);
            }
        }

        private enum Mode
        {
            Save,
            Import
        }

        private sealed class LocationView : DependencyObject
        {
            public readonly Location Location;

            public LocationView(Location location)
            {
                Location = location;

                string title = Location.Name;
                if (!string.IsNullOrEmpty(Location.Title))
                    title = title + " (" + Location.Title + ")";

                Title = title;
            }

            public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(LocationView), new PropertyMetadata(true));
            public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(LocationView), new PropertyMetadata(default(string)));

            public bool IsSelected
            {
                get { return (bool)GetValue(IsSelectedProperty); }
                set { SetValue(IsSelectedProperty, value); }
            }

            public string Title
            {
                get { return (string)GetValue(TitleProperty); }
                set { SetValue(TitleProperty, value); }
            }
        }
    }
}