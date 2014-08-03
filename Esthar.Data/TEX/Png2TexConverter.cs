using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class Png2TexConverter
    {
        private readonly string _inputFile;
        private readonly string _outputFile;

        public Png2TexConverter(string inputFile, string outputFile)
        {
            if (string.IsNullOrEmpty(inputFile))
                throw new ArgumentException("inputFile");
            if (string.IsNullOrEmpty(outputFile))
                throw new ArgumentException("outputFile");
            if (!File.Exists(inputFile))
                throw new FileNotFoundException(inputFile);

            _inputFile = inputFile;
            _outputFile = outputFile;
        }

        public void Convert()
        {
            BitmapPalette[] palettes = TryFindPalettes(_inputFile);

            using (Stream input = new FileStream(_inputFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream output = new FileStream(_outputFile, FileMode.Open, FileAccess.Write, FileShare.None))
            {
                PngBitmapDecoder decoder = new PngBitmapDecoder(input, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                BitmapFrame frame = decoder.Frames[0];

                TexHeader header = TexHeader.Create(frame, palettes);
                output.WriteStruct(header);

                if (palettes != null)
                    WriteIndexedImage(output, frame, palettes);
                else
                    WriteBgraImage(output, frame);
            }
        }

        private void WriteIndexedImage(Stream output, BitmapFrame frame, BitmapPalette[] palettes)
        {
            byte[] buff = new byte[4];
            foreach (Color color in palettes.SelectMany(p => p.Colors))
            {
                buff[0] = color.B;
                buff[1] = color.G;
                buff[2] = color.R;
                buff[3] = color.A;
                output.Write(buff, 0, buff.Length);
            }

            Int32Rect rect;
            using (SafeHGlobalHandle buffer = frame.GetPixels(out rect))
            {
                using (UnmanagedMemoryStream ums = new UnmanagedMemoryStream(buffer, buffer.Length, buffer.Length, FileAccess.Read))
                    ums.CopyTo(output, Math.Min(32 * 1024, buffer.Length));
            }
        }

        private void WriteBgraImage(Stream output, BitmapFrame frame)
        {
            throw new NotImplementedException();
        }

        private BitmapPalette[] TryFindPalettes(string inputFile)
        {
            string dir = Path.GetDirectoryName(inputFile);
            if (string.IsNullOrEmpty(dir))
                return null;

            string mask = Path.GetFileNameWithoutExtension(inputFile) + "*.act";
            string[] files = Directory.GetFiles(dir, mask, SearchOption.TopDirectoryOnly);
            if (files.Length == 0)
                return null;

            byte[] buff = new byte[4];
            BitmapPalette[] result = new BitmapPalette[files.Length];
            for (int i = 0; i < result.Length; i++)
                using (FileStream stream = new FileStream(files[i], FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    Color[] colors = new Color[256];
                    for (int c = 0; c < colors.Length; c++)
                    {
                        if (stream.Read(buff, 0, 3) != 3)
                            throw new Exception("Unexpected end of stream.");

                        colors[c] = Color.FromRgb(buff[0], buff[1], buff[2]);
                    }

                    if (stream.Position < stream.Length)
                    {
                        if (stream.Read(buff, 0, 4) != 4)
                            throw new Exception("Unexpected end of stream.");

                        if ((BitConverter.ToInt32(buff, 0) & 1) == 1)
                            colors[0].A = 0;
                    }

                    result[i] = new BitmapPalette(colors);
                }

            return result;
        }
    }
}