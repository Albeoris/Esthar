using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Esthar.Core;
using Esthar.UI;
using Image = System.Windows.Controls.Image;

namespace Esthar
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly DisposableStack _disposables = new DisposableStack();

        public MainWindow()
        {
            InitializeComponent();

            Closing += OnClosing;
            InitializeBottomPanel();
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainTabs.SafeDispose();
            _disposables.SafeDispose();
        }

        private void InitializeBottomPanel()
        {
            try
            {
                string gaemExecutablePath = Options.FindGameExecutablePath();
                if (!string.IsNullOrEmpty(gaemExecutablePath))
                    TryCreateButtonForFileSystemItem(gaemExecutablePath);

                TryCreateButtonForFileSystemItem(Options.WorkingDirectory);
                TryCreateButtonForFileSystemItem(Options.CVSDirectory);
            }
            catch (Exception ex)
            {
                UIHelper.ShowError(ex);
            }
        }

        private void TryCreateButtonForFileSystemItem(string path)
        {
            bool isDirectory = Directory.Exists(path);
            string extension = Path.GetExtension(path);

            if (!isDirectory && !String.Equals(extension, ".exe", StringComparison.OrdinalIgnoreCase))
                return;

            BitmapSource imageSource = ShellHelper.ExtractAssociatedIcon(path);
            Image image = new Image {Source = imageSource};
            Button button = new Button {Content = image, Tag = path, ToolTip = path, Style = (Style)Application.Current.Resources["XamlIconStyle"]};

            if (isDirectory) button.Click += OnDirectoryButtonClick;
            else button.Click += OnExecutableButtonClick;

            BottomPanel.Children.Add(button);
        }

        private void OnExecutableButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Button button = (Button)sender;
                string path = (string)button.Tag;

                Process process = new Process {StartInfo = new ProcessStartInfo(path)};
                process.Start();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError(ex);
            }
        }

        private void OnDirectoryButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Button button = (Button)sender;
                string path = (string)button.Tag;

                Process process = new Process {StartInfo = new ProcessStartInfo("explorer", path)};
                process.Start();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError(ex);
            }
        }
    }
}