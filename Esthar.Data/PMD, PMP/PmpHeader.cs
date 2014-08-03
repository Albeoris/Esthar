using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class PmpHeader
    {
        public const int Size = 516;

        public readonly ushort Unknown1;
        public readonly ushort Unknown2;
        public readonly BitmapPalette[] Palettes;

        public BitmapPalette this[int index]
        {
            get { return Palettes[index]; }
            set { Palettes[index] = value; }
        }

        private PmpHeader(ushort unknown1, ushort unknown2, BitmapPalette[] palettes)
        {
            Exceptions.CheckArgumentNull(palettes, "palettes");

            Unknown1 = unknown1;
            Unknown2 = unknown2;
            Palettes = palettes;
        }

        public static PmpHeader Read(Stream input)
        {
            ushort unknown1 = input.ReadStruct<ushort>();
            ushort unknown2 = input.ReadStruct<ushort>();

            BitmapPalette[] palettes = new BitmapPalette[16];
            for (int i = 0; i < palettes.Length; i++)
                palettes[i] = ReadPalette(input);

            return new PmpHeader(unknown1, unknown2, palettes);
        }

        private static BitmapPalette ReadPalette(Stream input)
        {
            byte[] buff = new byte[2];
            Color[] colors = new Color[16];

            for (int i = 0; i < colors.Length; i++)
                colors[i] = ColorsHelper.ReadColor(input, buff);

            return new BitmapPalette(colors);
        }
    }
}