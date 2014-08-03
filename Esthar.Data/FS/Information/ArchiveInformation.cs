using System;
using System.Collections.Generic;
using System.IO;
using Esthar.Core;

namespace Esthar.Data
{
    [Serializable]
    public sealed class ArchiveInformation
    {
        public string FilePath { get; private set; }
        public long FileSize { get; set; }
        public DateTime FileTime { get; set; }
        public bool IsOptimized { get; set; }

        public ArchiveArchiveEntry RootArchive { get; private set; }
        public ArchiveDirectoryEntry RootDirectory { get; private set; }

        public ArchiveInformation(string filePath, long fileSize, DateTime fileTime, bool isOptimized)
        {
            Exceptions.CheckArgumentNullOrEmprty(filePath, "filePath");
            if (fileSize < 0)
                throw new ArgumentOutOfRangeException("fileSize", fileSize, "Размер файла не может быть отрицательным.");

            FilePath = Path.ChangeExtension(filePath, null);
            FileSize = fileSize;
            FileTime = fileTime;
            IsOptimized = isOptimized;

            RootArchive = new ArchiveArchiveEntry(Path.GetFileNameWithoutExtension(FilePath));
            RootDirectory = new ArchiveDirectoryEntry("c:");
        }

        public string ContentFilePath
        {
            get { return FilePath + ".fs"; }
        }

        public string ListingFilePath
        {
            get { return FilePath + ".fl"; }
        }

        public string MetricFilePath
        {
            get { return FilePath + ".fi"; }
        }

        public ArchiveInformation GetTempArchiveInformation()
        {
            string tempPath = Path.Combine(Path.GetDirectoryName(FilePath), Guid.NewGuid().ToString());
            return new ArchiveInformation(tempPath, 0, DateTime.UtcNow, false);
        }

        public void MoveFiles(ArchiveInformation target)
        {
            File.Delete(target.ContentFilePath);
            File.Delete(target.MetricFilePath);

            File.Move(ContentFilePath, target.ContentFilePath);
            File.Move(MetricFilePath, target.MetricFilePath);

            FilePath = target.FilePath;
            RootArchive.Name = target.RootArchive.Name;
        }

        public void Update()
        {
            ArchiveInformationAccessor accessor = new ArchiveInformationAccessor(ContentFilePath);
            accessor.Write(this);
        }
    }
}