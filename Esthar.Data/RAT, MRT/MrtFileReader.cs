using System.IO;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class MrtFileReader : GameFileReader
    {
        public ushort[] Troops;

        public MrtFileReader(Stream input)
            : base(input)
        {
        }

        public override void Close()
        {
            IOStream.Seek(0, SeekOrigin.Begin);
            Troops = null;
        }

        public override void Open()
        {
            Close();
            Troops = IOStream.ReadStructsByTotalSize<ushort>(IOStream.Length);
        }
    }
}