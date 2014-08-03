using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using Esthar.Core;

namespace Esthar.Data
{
    [Serializable]
    public class ArchiveFileEntry : ArchiveEntryBase
    {
        public Compression Compression { get; set; }
        public int ContentOffset { get; set; }
        public int UncompressedContentSize { get; set; }
        public int ContentCapacity { get; set; }
        public int Index { get; set; }

        public ArchiveFileEntry(string name)
            : base(name, ArchiveEntryType.File)
        {
            Index = -1;
            Compression = Compression.None;
            ContentOffset = -1;
            UncompressedContentSize = -1;
            ContentCapacity = -1;
        }

        public long GetAbsoluteOffset()
        {
            long result = ContentOffset;
            ArchiveArchiveEntry parent = ParentArchive;
            while (parent != null && parent.ContentEntry != null)
            {
                result += parent.ContentEntry.ContentOffset;
                parent = parent.ParentArchive;
            }
            return result;
        }

        public FileSegment OpenReadableContentStream()
        {
            MemoryMappedFile mmf = ParentArchive.GetMemoryMappedFile(MemoryMappedFileAccess.Read);
            return new FileSegment(mmf, GetAbsoluteOffset(), UncompressedContentSize, MemoryMappedFileAccess.Read);
        }

        public FileSegment OpenWritableCapacityStream()
        {
            MemoryMappedFile mmf = ParentArchive.GetMemoryMappedFile(MemoryMappedFileAccess.ReadWrite);
            return new FileSegment(mmf, GetAbsoluteOffset(), ContentCapacity, MemoryMappedFileAccess.ReadWrite);
        }

        public void UpdateMetrics(int uncompressedContentSize, int contentOffset, Compression compression)
        {
            using (FileSegment stream = ParentArchive.GetMetricFileStream(MemoryMappedFileAccess.ReadWrite))
            using (BinaryWriter bw = stream.GetBinaryWriter())
            {
                stream.Seek(Index * 12, SeekOrigin.Begin);
                bw.Write(uncompressedContentSize);
                bw.Write(contentOffset);
                bw.Write((int)compression);
            }

            UncompressedContentSize = uncompressedContentSize;
            ContentOffset = contentOffset;
            Compression = compression;
        }

        public ArchiveFileEntry Clone()
        {
            return new ArchiveFileEntry(Name)
            {
                Index = Index,
                Compression = Compression,
                ContentOffset = ContentOffset,
                UncompressedContentSize = UncompressedContentSize,
                ContentCapacity = ContentCapacity
            };
        }
    }
}