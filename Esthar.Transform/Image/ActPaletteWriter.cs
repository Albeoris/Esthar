using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Esthar.Core;

namespace Esthar.Data.Transform
{
    public static class ActPaletteWriter
    {
        public static void Write(BitmapPalette[] palettes, string generalPath)
        {
            Exceptions.CheckArgumentNullOrEmprty(palettes, "palettes");
            Exceptions.CheckArgumentNullOrEmprty(generalPath, "generalPath");

            string prefix = Path.ChangeExtension(generalPath, null) + "_";

            for (int i = 0; i < palettes.Length; i++)
            {
                string filePath = prefix + i.ToString("D2") + ".act";
                Write(palettes[i], filePath);
            }
        }

        public static void Write(BitmapPalette palette, string filePath)
        {
            Exceptions.CheckArgumentNullOrEmprty(filePath, "filePath");

            using (Stream output = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                Write(palette, output);
        }

        public static void Write(BitmapPalette palette, Stream output)
        {
            Exceptions.CheckArgumentNull(palette, "palette");
            Exceptions.CheckArgumentNull(output, "output");

            byte[] buff = new byte[4];

            foreach (Color color in palette.Colors)
            {
                buff[0] = color.R;
                buff[1] = color.G;
                buff[2] = color.B;

                output.Write(buff, 0, 3);
            }

            buff[0] = buff[1] = buff[2] = 0;
            for (int i = 0; i < 256 - palette.Colors.Count; i++)
                output.Write(buff, 0, 3);

            if (palette.Colors[0].A == 0)
            {
                unsafe
                {
                    fixed (byte* ptr = buff)
                        *(int*)ptr = 1;
                }

                output.Write(buff, 0, 4);
            }
        }
    }
}