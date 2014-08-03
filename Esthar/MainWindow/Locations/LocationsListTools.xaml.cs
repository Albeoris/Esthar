using System;
using System.Linq;
using System.Windows;
using Esthar.Data.Transform;
using Esthar.UI;

namespace Esthar
{
    public partial class LocationsListTools
    {
        private Location[] _content;

        public LocationsListTools()
        {
            InitializeComponent();
        }

        public void SetContent(Location[] content)
        {
            _content = content;
        }

        private void OnSaveButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                LocationSaveDialog.ShowSave(_content.Where(l => l.SaveRequest != LocationProperty.None));
            }
            catch (Exception ex)
            {
                UIHelper.ShowError(ex);
            }
        }

        private void OnUploadButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                LocationSaveDialog.ShowImport(_content.Where(l => l.Importable != LocationProperty.None && l.SaveRequest == LocationProperty.None));
            }
            catch (Exception ex)
            {
                UIHelper.ShowError(ex);
            }
        }
    }
}