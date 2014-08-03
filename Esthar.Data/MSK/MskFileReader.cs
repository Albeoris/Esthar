using System.IO;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class MskFileReader : GameFileReader
    {
        public MskRect[] Rects;

        public MskFileReader(Stream input)
            : base(input)
        {
        }

        public override void Close()
        {
            IOStream.Seek(0, SeekOrigin.Begin);
            Rects = null;
        }

        public override void Open()
        {
            Close();

            int count = IOStream.ReadStruct<int>();
            Rects = IOStream.ReadStructs<MskRect>(count);
        }
    }
}