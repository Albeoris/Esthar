using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Esthar.Core;
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

        private void OnExportButtonClick(object sender, RoutedEventArgs e)
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

        private void OnInboxButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                int filesCopied = 0;
                DirectoryInfo source = new DirectoryInfo(Options.WorkingDirectory);
                DirectoryInfo target = new DirectoryInfo(Options.CVSDirectory);
                SyncFolder(source, target, ref filesCopied);
                AfterSyncFolder(source, target, filesCopied);
            }
            catch (Exception ex)
            {
                UIHelper.ShowError(ex);
            }
        }

        private void OnOutboxButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                int filesCopied = 0;
                DirectoryInfo source = new DirectoryInfo(Options.CVSDirectory);
                DirectoryInfo target = new DirectoryInfo(Options.WorkingDirectory);
                SyncFolder(source, target, ref filesCopied);
                AfterSyncFolder(source,target,filesCopied);
            }
            catch (Exception ex)
            {
                UIHelper.ShowError(ex);
            }
        }

        private void SyncFolder(DirectoryInfo source, DirectoryInfo target, ref int filesCount)
        {
            FileInfo[] sourceFiles = source.GetFiles();
            FileInfo[] targetFiles = target.GetFiles();

            foreach (FileInfo sourceFile in sourceFiles)
            {
                if (sourceFile.Name[0] == '.')
                    continue;

                FileInfo targetFile = targetFiles.FirstOrDefault(tf => tf.Name.Equals(sourceFile.Name, StringComparison.OrdinalIgnoreCase));
                if (targetFile == null || targetFile.LastWriteTimeUtc != sourceFile.LastWriteTimeUtc || sourceFile.Length != targetFile.Length)
                {
                    sourceFile.CopyTo(Path.Combine(target.FullName, sourceFile.Name));
                    filesCount++;
                }
            }

            DirectoryInfo[] sourceDirectories = source.GetDirectories();
            foreach (DirectoryInfo sourceDirectory in sourceDirectories)
            {
                if(sourceDirectory.Name[0] == '.')
                    continue;

                SyncFolder(sourceDirectory, target.CreateSubdirectory(sourceDirectory.Name), ref filesCount);
            }
        }

        private void AfterSyncFolder(DirectoryInfo source, DirectoryInfo target, int filesCopied)
        {
            StringBuilder sb = new StringBuilder(1024);
            
            sb.AppendLine("Изменения из каталога:");
            sb.AppendLine(source.FullName);
            sb.AppendLine();
            
            sb.AppendLine("Успешно перенесены в каталог:");
            sb.AppendLine(target.FullName);
            sb.AppendLine();

            sb.AppendFormat("Количество измененных файлов: {0}", filesCopied);
            MessageBox.Show(sb.ToString(), "Синхронизация", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}