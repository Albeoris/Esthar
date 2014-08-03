using System;
using System.IO;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Esthar.Core;
using Esthar.OpenGL;

namespace Esthar.Data
{
    public sealed class Tex2PngConverter
    {
        private readonly string _inputFile;
        private readonly string _outputFile;

        public Tex2PngConverter(string inputFile, string outputFile)
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
            using (Stream input = new FileStream(_inputFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                TexHeader header = TexHeader.Read(input);
                BitmapPalette[] palettes = header.TryReadPalette(input);

                if (palettes.Length == 0)
                {
                    using (Stream output = new FileStream(_outputFile, FileMode.Create, FileAccess.Write, FileShare.None))
                        Convert(input, output, header);
                }
                else
                {
                    using (Stream output = new FileStream(_outputFile, FileMode.Create, FileAccess.Write, FileShare.None))
                        Convert(input, output, header, palettes[0]);

                    for (int i = 0; i < palettes.Length; i++)
                    {
                        string fileName = Path.ChangeExtension(_outputFile, string.Empty) + "_" + i.ToString("D" + palettes.Length / 10 + 1) + ".act";
                        using (Stream output = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
                            SavePalette(palettes[i], output);
                    }
                }
            }
        }

        private void SavePalette(BitmapPalette bitmapPalette, Stream output)
        {
            //if (bitmapPalette.Colors.Count != 256)
            //    throw new ArgumentException("bitmapPalette");

            byte[] buff = new byte[4];
            
            foreach (Color color in bitmapPalette.Colors)
            {
                buff[0] = color.R;
                buff[1] = color.G;
                buff[2] = color.B;

                output.Write(buff, 0, 3);
            }

            buff[0] = buff[1] = buff[2] = 0;
            for (int i = 0; i < 256 - bitmapPalette.Colors.Count; i++)
                output.Write(buff, 0, 3);

            if (bitmapPalette.Colors[0].A == 0)
            {
                unsafe
                {
                    fixed (byte* ptr = buff)
                        *(int*)ptr = 1;
                }
                
                output.Write(buff, 0, 4);
            }
        }

        private void Convert(Stream input, Stream output, TexHeader header, BitmapPalette palette)
        {
            using (GLTexture layer = header.TryReadData(input))
            {
                throw new NotImplementedException();
                //BitmapSource source = BitmapSource.Create(header.ImageWidth, header.ImageHeight, 96, 96, PixelFormats.Indexed8, palette, data.DangerousGetHandle(), (int)data.ByteLength, header.ImageWidth);

                //PngBitmapEncoder encoder = new PngBitmapEncoder { Interlace = PngInterlaceOption.Off };
                //encoder.Frames.Add(BitmapFrame.Create(source));
                //encoder.Save(output);
            }
        }

        private void Convert(Stream input, Stream output, TexHeader header)
        {
            using (GLTexture layer = header.TryReadData(input))
            {
                throw new NotImplementedException();
                //TexPixelFormatsConverter converter = new TexPixelFormatsConverter(header, data, PixelFormats.Bgra32);
                //using (SafeHGlobalHandle result = converter.Convert())
                //{
                //    data.Dispose();
                //    BitmapSource source = BitmapSource.Create(header.ImageWidth, header.ImageHeight, 96, 96, PixelFormats.Bgra32, null, result.DangerousGetHandle(), (int)result.ByteLength, header.ImageWidth * 4);
                    
                //    PngBitmapEncoder encoder = new PngBitmapEncoder { Interlace = PngInterlaceOption.Off };
                //    encoder.Frames.Add(BitmapFrame.Create(source));
                //    encoder.Save(output);
                //}
            }
        }
    }
}