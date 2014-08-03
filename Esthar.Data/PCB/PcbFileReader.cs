using System.IO;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class PcbFileReader : GameFileReader
    {
        public PcbEntry[] Entries;

        public PcbFileReader(Stream input)
            : base(input)
        {
        }

        public override void Close()
        {
            IOStream.Seek(0, SeekOrigin.Begin);
            Entries = null;
        }

        public override void Open()
        {
            Close();
            Entries = IOStream.ReadStructsByTotalSize<PcbEntry>(IOStream.Length);
        }
    }
}