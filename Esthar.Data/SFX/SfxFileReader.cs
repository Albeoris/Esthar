using System.IO;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class SfxFileReader : GameFileReader
    {
        public uint[] SoundIds;

        public SfxFileReader(Stream input)
            : base(input)
        {
        }

        public override void Close()
        {
            IOStream.Seek(0, SeekOrigin.Begin);
            SoundIds = null;
        }

        public override void Open()
        {
            Close();
            SoundIds = IOStream.ReadStructsByTotalSize<uint>(IOStream.Length);
        }
    }
}