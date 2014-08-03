using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class TimPalettes
    {
        public readonly TimPalettesHeader PalettesHeader;
        public readonly BitmapPalette[] Palettes;

        public BitmapPalette this[int index]
        {
            get { return Palettes[index]; }
            set { Palettes[index] = value; }
        }

        private TimPalettes(TimPalettesHeader palettesHeader, BitmapPalette[] palettes)
        {
            Exceptions.CheckArgumentNull(palettesHeader, "palettesHeader");
            Exceptions.CheckArgumentNull(palettes, "palettes");

            PalettesHeader = palettesHeader;
            Palettes = palettes;
        }

        public static TimPalettes TryRead(TimHeader header, Stream input)
        {
            Exceptions.CheckArgumentNull(header, "header");
            Exceptions.CheckArgumentNull(input, "input");

            if (!header.HasPalette)
                return null;

            return Read(header, input);
        }

        private static TimPalettes Read(TimHeader header, Stream input)
        {
            TimPalettesHeader palettesHeader = TimPalettesHeader.Read(input);
            BitmapPalette[] palettes = new BitmapPalette[palettesHeader.PalettesCount];
            for (int i = 0; i < palettes.Length; i++)
                palettes[i] = ReadPalette(palettesHeader, input);
            return new TimPalettes(palettesHeader, palettes);
        }

        private static BitmapPalette ReadPalette(TimPalettesHeader palettesHeader, Stream input)
        {
            byte[] buff = new byte[2];
            Color[] colors = new Color[palettesHeader.ColorsPerPalette];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = ColorsHelper.ReadColor(input, buff);
            return new BitmapPalette(colors);
        }
    }
}