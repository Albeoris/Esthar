using System.IO;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class RatFileReader : GameFileReader
    {
        public uint? Rates;

        public RatFileReader(Stream input)
            : base(input)
        {
        }

        public override void Close()
        {
            IOStream.Seek(0, SeekOrigin.Begin);
            Rates = null;
        }

        public override void Open()
        {
            Close();
            Rates = IOStream.ReadStruct<uint>();
        }
    }
}