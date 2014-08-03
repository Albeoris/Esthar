using System;
using System.Windows;
using Esthar.Data.Transform;
using Esthar.Font;
using Esthar.UI;

namespace Esthar
{
    /// <summary>
    /// Логика взаимодействия для MainMenu.xaml
    /// </summary>
    public partial class MainMenu
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private void ShowFontConfigWindow(object sender, RoutedEventArgs e)
        {
            try
            {
                FontConfigurationWindow window = new FontConfigurationWindow(GameFont.HiResFont);
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError(ex);
            }
        }

        private void ShowTagsConfigWindow(object sender, RoutedEventArgs e)
        {
            try
            {
                TagsConfigWindow window = new TagsConfigWindow();
                if (window.ShowDialog() == true)
                    UiService.InvokeTagsChanged();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError(ex);
            }
        }
    }
}
