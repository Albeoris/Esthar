using System.Runtime.InteropServices;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public sealed class MimSimpleTile
    {
        public short X, Y, SourceX, SourceY, Z;
        public byte Flags1;
        public byte Unknown;
        public ushort Flags2;
        public byte Parameter;
        public byte State;

        public byte TextureID
        {
            get { return (byte)(Flags1 & 15); }
        }

        public byte Blend1
        {
            get { return (byte)((Flags1 >> 4) & 1); }
        }

        public byte Blend2
        {
            get { return (byte)((Flags1 >> 5) & 7); }
        }
        
        public byte PaletteId
        {
            get { return (byte)((Flags2 >> 6) & 15); }
        }
    }
}