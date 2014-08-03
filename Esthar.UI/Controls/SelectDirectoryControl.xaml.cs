using System;
using System.IO;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Esthar.UI
{
    public partial class SelectDirectoryControl
    {
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(SelectDirectoryControl), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(SelectDirectoryControl), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty MustExistsProperty = DependencyProperty.Register("MustExists", typeof(bool), typeof(SelectDirectoryControl), new PropertyMetadata(default(bool)));
        public static readonly DependencyProperty CreateIfNotExistsProperty = DependencyProperty.Register("CreateIfNotExists", typeof(bool), typeof(SelectDirectoryControl), new PropertyMetadata(default(bool)));

        public SelectDirectoryControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public string Path
        {
            get { return (string)GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }

        public bool MustExists
        {
            get { return (bool)GetValue(MustExistsProperty); }
            set { SetValue(MustExistsProperty, value); }
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                using (CommonOpenFileDialog dlg = new CommonOpenFileDialog(Header))
                {
                    dlg.IsFolderPicker = true;
                    dlg.EnsurePathExists = MustExists;

                    string currentPath = Path;

                    if (!string.IsNullOrEmpty(currentPath) && Directory.Exists(currentPath))
                        dlg.InitialDirectory = currentPath;

                    if (dlg.ShowDialog(Window.GetWindow(this)) != CommonFileDialogResult.Ok)
                        return;

                    string folder = dlg.FileName;
                    if (MustExists && !Directory.Exists(folder))
                        throw new DirectoryNotFoundException(folder);

                    Path = folder;
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError(ex);
            }
        }
    }
}