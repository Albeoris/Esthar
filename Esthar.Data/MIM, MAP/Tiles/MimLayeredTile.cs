using System.Runtime.InteropServices;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public sealed class MimLayeredTile
    {
        public short X, Y, Z;
        public byte Flags1;
        public byte Unknown;
        public ushort Flags2;
        public byte SourceX, SourceY;
        public byte Flags3;
        public MimTileBlendType BlendType;
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

        public int LayerId
        {
            get { return (byte)((Flags3 >> 7) & 127); }
            set { Flags3 |= (byte)((value & 127) << 7); }
        }

        public bool IsFullByteIndices
        {
            get { return Blend2 > 3; }
        }

        public int GetPositionInTexture()
        {
            return TextureID * 128 + SourceY * MimTextures.AtlasWidth + SourceX / (IsFullByteIndices ? 1 : 2);
        }

        public int GetPositionInImage(MapFileReader map, int strade)
        {
            return (Y - map.MinY) * strade + (X - map.MinX) * 3;
        }
    }
}