using System.IO;
using System.Runtime.InteropServices;
using Esthar.Core;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public sealed class TimImageHeader
    {
        public int Size;
        public short X;
        public short Y;
        public short Width;
        public short Height;

        public int ContentSize
        {
            get { return Size - Marshal.SizeOf(this); }
        }

        public static TimImageHeader Read(TimHeader header, Stream input)
        {
            TimImageHeader result = input.ReadStruct<TimImageHeader>();
            if (header.BytesPerPixel == 0)
                result.Width *= 2;
            return result;
        }
    }
}