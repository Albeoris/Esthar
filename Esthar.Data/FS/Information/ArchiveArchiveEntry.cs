using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using Esthar.Core;

namespace Esthar.Data
{
    [Serializable]
    public sealed class ArchiveArchiveEntry : ArchiveEntryBase
    {
        public readonly List<ArchiveEntryBase> Childs;

        public ArchiveFileEntry ContentEntry { get; set; }
        public ArchiveFileEntry ListingEntry { get; set; }
        public ArchiveFileEntry MetricsEntry { get; set; }

        public int CurrentPosition { get; set; }

        public ArchiveArchiveEntry(string name)
            : base(name, ArchiveEntryType.Archive)
        {
            Childs = new List<ArchiveEntryBase>();
        }

        public bool IsComplete
        {
            get
            {
                if (ContentEntry == null || ListingEntry == null || MetricsEntry == null)
                    return false;

                CheckValid();
                return true;
            }
        }

        public void AddEntry(ArchiveEntryBase entry)
        {
            if (entry.Type == ArchiveEntryType.Directory)
                throw new ArgumentException("entry");

            entry.ParentArchive = this;
            Childs.Add(entry);
        }

        public bool IsUncompressed()
        {
            foreach (ArchiveEntryBase child in Childs)
            {
                if (child.Type == ArchiveEntryType.Archive)
                {
                    ArchiveArchiveEntry archiveEntry = (ArchiveArchiveEntry)child;
                    if (archiveEntry.MetricsEntry.Compression != Compression.None
                        || archiveEntry.ListingEntry.Compression != Compression.None
                        || archiveEntry.ContentEntry.Compression != Compression.None)
                        return false;
                }
                else if (child.Type == ArchiveEntryType.File)
                {
                    ArchiveFileEntry fileEntry = (ArchiveFileEntry)child;
                    if (fileEntry.Compression != Compression.None)
                        return false;
                }
            }
            
            return true;
        }

        public List<ArchiveFileEntry> Expand()
        {
            List<ArchiveFileEntry> result = new List<ArchiveFileEntry>(32);
            result.AddRange(Childs.OfType<ArchiveFileEntry>());
            result.AddRange(Childs.OfType<ArchiveArchiveEntry>().Select(e => e.MetricsEntry));
            result.AddRange(Childs.OfType<ArchiveArchiveEntry>().Select(e => e.ListingEntry));
            result.AddRange(Childs.OfType<ArchiveArchiveEntry>().SelectMany(e => e.Expand()));
            return result;
        }

        public MemoryMappedFile GetMemoryMappedFile(MemoryMappedFileAccess access)
        {
            ArchiveArchiveEntry archive = this;
            while (archive.ParentArchive != null)
                archive = archive.ParentArchive;

            string path = Path.Combine(Options.GameDataDirectoryPath, archive.Name + ".fs");
            FileStream stream = new FileStream(path, FileMode.Open, ConvertAccess(access), FileShare.ReadWrite);
            try
            {
                return MemoryMappedFile.CreateFromFile(stream, null, 0, access, null, HandleInheritability.Inheritable, false);
            }
            catch
            {
                stream.SafeDispose();
                throw;
            }
        }

        public FileSegment GetMetricFileStream(MemoryMappedFileAccess access)
        {
            if (ParentArchive != null)
            {
                MemoryMappedFile mmf = GetMemoryMappedFile(access);
                return new FileSegment(mmf, MetricsEntry.GetAbsoluteOffset(), MetricsEntry.UncompressedContentSize, access);
            }

            string path = Path.Combine(Options.GameDataDirectoryPath, Name + ".fi");
            FileStream stream = new FileStream(path, FileMode.Open, ConvertAccess(access), FileShare.ReadWrite);
            using (DisposableAction insurance = new DisposableAction(stream.Dispose))
            {
                MemoryMappedFile mmf =  MemoryMappedFile.CreateFromFile(stream, null, 0, access, null, HandleInheritability.Inheritable, false);
                
                insurance.Cancel();
                return new FileSegment(mmf, 0, stream.Length, access);
            }
        }

        private void CheckValid()
        {
            // ReSharper disable PossibleNullReferenceException
            int h1 = Path.GetFileNameWithoutExtension(ContentEntry.Name).GetHashCode();
            int h2 = Path.GetFileNameWithoutExtension(ListingEntry.Name).GetHashCode();
            int h3 = Path.GetFileNameWithoutExtension(MetricsEntry.Name).GetHashCode();
            if (h1 != h2 || h2 != h3)
                throw new Exception("Ошибка при сборе сведений об архиве.");
            // ReSharper restore PossibleNullReferenceException
        }

        private static FileAccess ConvertAccess(MemoryMappedFileAccess access)
        {
            switch (access)
            {
                case MemoryMappedFileAccess.CopyOnWrite:
                case MemoryMappedFileAccess.ReadWrite:
                case MemoryMappedFileAccess.ReadWriteExecute:
                    return FileAccess.ReadWrite;
                case MemoryMappedFileAccess.Read:
                    return FileAccess.Read;
                case MemoryMappedFileAccess.Write:
                    return FileAccess.Write;
            }

            throw new NotImplementedException();
        }

        public SortedList<int, ArchiveFileEntry> GetOrderedFileEntries()
        {
            SortedList<int, ArchiveFileEntry> list = new SortedList<int, ArchiveFileEntry>();

            foreach (ArchiveEntryBase entry in Childs)
            {
                switch (entry.Type)
                {
                    case ArchiveEntryType.Archive:
                    {
                        ArchiveArchiveEntry archiveEntry = (ArchiveArchiveEntry)entry;
                        list.Add(archiveEntry.MetricsEntry.Index, archiveEntry.MetricsEntry);
                        list.Add(archiveEntry.ListingEntry.Index, archiveEntry.ListingEntry);
                        list.Add(archiveEntry.ContentEntry.Index, archiveEntry.ContentEntry);
                        break;
                    }
                    case ArchiveEntryType.File:
                    {
                        ArchiveFileEntry fileEntry = (ArchiveFileEntry)entry;
                        list.Add(fileEntry.Index, fileEntry);
                        break;
                    }
                }
            }

            return list;
        }
    }
}