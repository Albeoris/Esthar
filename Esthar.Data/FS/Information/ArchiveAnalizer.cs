using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class ArchiveAnalizer
    {
        private readonly ArchiveInformation _info;
        private readonly ConcurrentDictionary<string, ArchiveArchiveEntry> _childArchives = new ConcurrentDictionary<string, ArchiveArchiveEntry>();

        public ArchiveAnalizer(ArchiveInformation info)
        {
            Exceptions.CheckArgumentNull(info, "info");

            _info = info;
        }

        public void ReadNativeInformation()
        {
            using (FileStream ms = new FileStream(_info.MetricFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (FileStream ls = new FileStream(_info.ListingFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                ReadInformation(_info.RootArchive, ms, ls);
        }

        public void AnalizeUncompressedArchive(MemoryMappedFile memoryFile)
        {
            foreach (ArchiveArchiveEntry archive in _info.RootArchive.Childs.OfType<ArchiveArchiveEntry>())
                AnalizeUncompressedArchive(memoryFile, archive, 0);
        }

        private void AnalizeUncompressedArchive(MemoryMappedFile memoryFile, ArchiveArchiveEntry archive, long parentOffset)
        {
            using (MemoryMappedViewStream metricsStream = memoryFile.CreateViewStream(parentOffset + archive.MetricsEntry.ContentOffset, archive.MetricsEntry.UncompressedContentSize))
            using (MemoryMappedViewStream listingStream = memoryFile.CreateViewStream(parentOffset + archive.ListingEntry.ContentOffset, archive.ListingEntry.UncompressedContentSize))
            {
                ReadInformation(archive, metricsStream, listingStream);
                foreach (ArchiveArchiveEntry child in archive.Childs.OfType<ArchiveArchiveEntry>())
                    AnalizeUncompressedArchive(memoryFile, child, parentOffset + archive.ContentEntry.ContentOffset);
            }
        }

        private void ReadInformation(ArchiveArchiveEntry parentArchive, Stream metricsStream, Stream listingStream)
        {
            _childArchives.Clear();

            using (BinaryReader mbr = new BinaryReader(metricsStream))
            using (StreamReader lsr = new StreamReader(listingStream, Encoding.GetEncoding(1251)))
            {
                int index = 0;
                while (!lsr.EndOfStream)
                {
                    string path = lsr.ReadLine();
                    if (string.IsNullOrEmpty(path))
                        break;

                    int uncompressedContentSize = mbr.ReadInt32();
                    int contentOffset = mbr.ReadInt32();
                    int compression = mbr.ReadInt32();

                    ArchiveFileEntry entry = new ArchiveFileEntry(Path.GetFileName(path))
                    {
                        Index = index++,
                        UncompressedContentSize = uncompressedContentSize,
                        ContentOffset = contentOffset,
                        Compression = (Compression)compression
                    };

                    if (TryAddChildArchive(path, entry, parentArchive))
                        continue;

                    parentArchive.AddEntry(entry);
                    _info.RootDirectory.AddEntry(path, entry);
                }
            }
        }

        private bool TryAddChildArchive(string path, ArchiveFileEntry entry, ArchiveArchiveEntry parentArchive)
        {
            Action<ArchiveArchiveEntry, ArchiveFileEntry> valueSetter;
            switch (Path.GetExtension(entry.Name))
            {
                case ".fs":
                    valueSetter = (a, e) => a.ContentEntry = e;
                    break;
                case ".fl":
                    valueSetter = (a, e) => a.ListingEntry = e;
                    break;
                case ".fi":
                    valueSetter = (a, e) => a.MetricsEntry = e;
                    break;
                default:
                    return false;
            }

            path = Path.ChangeExtension(path, null);
            ArchiveArchiveEntry archive = _childArchives.GetOrAdd(path, p => new ArchiveArchiveEntry(Path.ChangeExtension(entry.Name, null)));
            valueSetter(archive, entry);

            if (archive.IsComplete)
            {
                _childArchives.TryRemove(path, out archive);
                parentArchive.AddEntry(archive);
            }

            return true;
        }
    }
}