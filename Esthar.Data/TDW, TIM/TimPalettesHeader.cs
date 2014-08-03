using System.IO;
using System.Runtime.InteropServices;
using Esthar.Core;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public sealed class TimPalettesHeader
    {
        public readonly int Size;
        public readonly short X;
        public readonly short ImageHeight;
        public readonly short PalettesCount;
        public readonly short ColorsPerPalette;

        public int ContentSize
        {
            get { return Size - Marshal.SizeOf(this); }
        }

        public static TimPalettesHeader Read(Stream input)
        {
            return input.ReadStruct<TimPalettesHeader>();
        }
    }
}