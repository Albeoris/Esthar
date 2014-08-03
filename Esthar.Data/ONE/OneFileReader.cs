using System.IO;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class OneFileReader : GameFileReader
    {
        public OneFileReader(Stream input)
            : base(input)
        {
        }

        public override void Close()
        {
            IOStream.Seek(0, SeekOrigin.Begin);
        }

        public override void Open()
        {
            Close();
        }

        public SafeHGlobalHandle ReadData()
        {
            return IOStream.ReadBuff((int)IOStream.Length);
        }
    }
}