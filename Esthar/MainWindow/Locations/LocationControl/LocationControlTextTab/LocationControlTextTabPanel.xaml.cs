using System;
using System.Windows;
using System.Windows.Controls;
using Esthar.Core;
using Esthar.UI;

namespace Esthar
{
    /// <summary>
    /// Логика взаимодействия для LocationControlTextTabList.xaml
    /// </summary>
    public partial class LocationControlTextTabPanel
    {
        public LocationTextControlContext LocationContext { get; set; }

        public LocationControlTextTabPanel()
        {
            InitializeComponent();
        }

        private void OnCharacterButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                LocationContext.InsertCharater((FF8TextTagCharacter)((Button)sender).Tag);
            }
            catch (Exception ex)
            {
                UIHelper.ShowError(ex);
            }
        }
    }
}