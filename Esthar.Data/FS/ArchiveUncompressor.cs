using System;
using System.IO;
using System.Linq;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class ArchiveUncompressor
    {
        private readonly ArchiveInformation _info;
        private readonly Stream _input;
        private readonly Stream _output;

        private readonly byte[] _buff = new byte[4096];

        public event EventHandler<ProgressArgs> Progress;

        public ArchiveUncompressor(ArchiveInformation info, Stream input, Stream output)
        {
            Exceptions.CheckArgumentNull(info, "info");
            Exceptions.CheckArgumentNull(input, "input");
            Exceptions.CheckArgumentNull(output, "output");

            _info = info;
            _input = input;
            _output = output;
        }

        public void Uncompress()
        {
            foreach (ArchiveArchiveEntry archive in _info.RootArchive.Childs.OfType<ArchiveArchiveEntry>())
                Uncompress(archive.MetricsEntry);
            foreach (ArchiveArchiveEntry archive in _info.RootArchive.Childs.OfType<ArchiveArchiveEntry>())
                Uncompress(archive.ListingEntry);
            foreach (ArchiveArchiveEntry archive in _info.RootArchive.Childs.OfType<ArchiveArchiveEntry>())
                Uncompress(archive.ContentEntry);
            foreach (ArchiveFileEntry file in _info.RootArchive.Childs.OfType<ArchiveFileEntry>())
                Uncompress(file);
        }

        private void Uncompress(ArchiveFileEntry file)
        {
            _input.Position = file.ContentOffset;
            file.ContentOffset = (int)_output.Position;

            Progress.NullSafeInvoke(this, new ProgressArgs(file, 0, file.UncompressedContentSize));
            if (file.Compression == Compression.None)
            {
                _input.CopyTo(_output, file.UncompressedContentSize, _buff);
            }
            else
            {
                _input.Seek(4, SeekOrigin.Current);
                LZSStream lzs = new LZSStream(_input, _output);
                lzs.Decompress(file.UncompressedContentSize);
            }
            Progress.NullSafeInvoke(this, new ProgressArgs(file, file.UncompressedContentSize, file.UncompressedContentSize));
        }

        public static int CalcUncompressedSize(ArchiveInformation info)
        {
            int result = 0;

            foreach (ArchiveArchiveEntry archive in info.RootArchive.Childs.OfType<ArchiveArchiveEntry>())
                result += archive.MetricsEntry.UncompressedContentSize;
            foreach (ArchiveArchiveEntry archive in info.RootArchive.Childs.OfType<ArchiveArchiveEntry>())
                result += archive.ListingEntry.UncompressedContentSize;
            foreach (ArchiveArchiveEntry archive in info.RootArchive.Childs.OfType<ArchiveArchiveEntry>())
                result += archive.ContentEntry.UncompressedContentSize;
            foreach (ArchiveFileEntry file in info.RootArchive.Childs.OfType<ArchiveFileEntry>())
                result += file.UncompressedContentSize;

            return result;
        }
    }
}