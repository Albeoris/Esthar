using System.IO;
using System.IO.MemoryMappedFiles;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class CraftingBinFileReader : GameFileReader
    {
        public CraftingBinEntry[] Entries;

        public CraftingBinFileReader(Stream input)
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

            IOStream.Seek(0, SeekOrigin.Begin);
            Entries = IOStream.ReadStructsByTotalSize<CraftingBinEntry>(IOStream.Length);
        }
    }
}