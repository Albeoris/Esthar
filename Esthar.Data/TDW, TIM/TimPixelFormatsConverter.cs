using System;
using System.IO;
using System.Windows.Media;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class TimPixelFormatsConverter
    {
        public static void Convert(TimHeader header, TimImageHeader imageHeader, Stream input, Stream output)
        {
            TimPixelFormatsConverter converter = new TimPixelFormatsConverter(header, imageHeader, input, output);
            converter.Convert();
        }

        private readonly TimHeader _header;
        private readonly TimImageHeader _imageHeader;
        private readonly Stream _input;
        private readonly Stream _output;

        public TimPixelFormatsConverter(TimHeader header, TimImageHeader imageHeader, Stream input, Stream output)
        {
            Exceptions.CheckArgumentNull(header, "header");
            Exceptions.CheckArgumentNull(imageHeader, "imageHeader");
            Exceptions.CheckArgumentNull(input, "input");
            Exceptions.CheckArgumentNull(output, "output");

            _header = header;
            _imageHeader = imageHeader;
            _input = input;
            _output = output;
        }

        public void Convert()
        {
            switch (_header.BytesPerPixel)
            {
                case 0:
                case 1:
                    CopyIndexed();
                    break;
                case 2:
                    ConvertDouble();
                    break;
                case 3:
                    ConvertTriple();
                    break;
            }
        }

        private void CopyIndexed()
        {
            _input.CopyTo(_output, _imageHeader.ContentSize, new byte[16384]);
        }

        private void ConvertDouble()
        {
            byte[] buff = new byte[2];
            for (int i = 0; i < _imageHeader.ContentSize; i += 2)
            {
                Color color = ColorsHelper.ReadColor(_input, buff);
                ColorsHelper.WriteBgra(_output, color);
            }
        }

        private void ConvertTriple()
        {
            byte[] buff = new byte[3];
            for (int i = 0; i < _imageHeader.ContentSize; i += 3)
            {
                Color color = ColorsHelper.ReadColor(_input, buff);
                ColorsHelper.WriteBgra(_output, color);
            }
        }
    }
}