using System.IO;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class PmdFileReader : GameFileReader
    {
        public PmdEntry1[] Unknown1; // 16x372
        public PmdEntry2[] Unknown2; // 16x254

        public PmdFileReader(Stream input)
            : base(input)
        {
        }

        public override void Close()
        {
            IOStream.Seek(0, SeekOrigin.Begin);
            Unknown1 = null;
            Unknown2 = null;
        }

        public override void Open()
        {
            Close();

            if (IOStream.Length == 4 || IOStream.Length == 0)
                return;

            Unknown1 = IOStream.ReadStructs<PmdEntry1>(16);
            Unknown2 = IOStream.ReadStructs<PmdEntry2>(16);
        }
    }
}