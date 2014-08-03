using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class MimPalettes
    {
        public const int Size = 16 * 256 * 2;

        public readonly BitmapPalette[] Palettes;

        public BitmapPalette this[int index]
        {
            get { return Palettes[index]; }
            set { Palettes[index] = value; }
        }

        private MimPalettes(BitmapPalette[] palettes)
        {
            Exceptions.CheckArgumentNull(palettes, "palettes");
            
            Palettes = palettes;
        }

        public static MimPalettes Read(Stream input)
        {
            BitmapPalette[] palettes = new BitmapPalette[16];
            for (int i = 0; i < palettes.Length; i++)
                palettes[i] = ReadPalette(input);
            return new MimPalettes(palettes);
        }

        private static BitmapPalette ReadPalette(Stream input)
        {
            byte[] buff = new byte[2];
            Color[] colors = new Color[256];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = ColorsHelper.ReadColor(input, buff);
            return new BitmapPalette(colors);
        }
    }
}