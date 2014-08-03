using System.IO;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class PvpFileReader : GameFileReader
    {
        public uint? Value;

        public PvpFileReader(Stream input)
            : base(input)
        {
        }

        public override void Close()
        {
            IOStream.Seek(0, SeekOrigin.Begin);
            Value = null;
        }

        public override void Open()
        {
            Close();
            Value = IOStream.ReadStruct<uint>();
        }
    }
}