using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Esthar.Core;

namespace Esthar.Data.Transform
{
    public static class ActPaletteReader
    {
        public static BitmapPalette[] Find(string generalName)
        {
            Exceptions.CheckArgumentNullOrEmprty(generalName, "generalName");

            string dir = Path.GetDirectoryName(generalName);
            if (string.IsNullOrEmpty(dir))
                return null;

            string mask = Path.GetFileNameWithoutExtension(generalName) + "*.act";
            string[] files = Directory.GetFiles(dir, mask, SearchOption.TopDirectoryOnly);
            if (files.Length == 0)
                return null;

            BitmapPalette[] result = new BitmapPalette[files.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = Read(files[i]);

            return result;
        }

        public static BitmapPalette[] Read(string dirPath, string generalName)
        {
            string[] files = Directory.GetFiles(dirPath, String.Format("{0}_*.act", generalName));
            if (files.Length == 0)
                return null;

            BitmapPalette[] result = new BitmapPalette[files.Length];

            for (int i = 0; i < files.Length; i++)
                result[i] = Read(files[i]);

            return result;
        }

        private static BitmapPalette Read(string fullPath)
        {
            using (FileStream input = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                return Read(input);
        }

        private static BitmapPalette Read(FileStream input)
        {
            byte[] buff = new byte[4];
            Color[] colors = new Color[256];

            for (int i = 0; i < colors.Length; i++)
            {
                input.EnsureRead(buff, 0, 3);
                colors[i] = Color.FromRgb(buff[0], buff[1], buff[2]);
            }

            if (!input.IsEndOfStream())
            {
                input.EnsureRead(buff, 0, 4);
                if (BitConverter.ToInt32(buff, 0) == 1)
                    colors[0].A = 0;
            }

            return new BitmapPalette(colors);
        }
    }
}