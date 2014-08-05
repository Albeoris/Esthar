using System;
using System.Drawing.Imaging;
using System.IO;
using Esthar.Core;
using Esthar.OpenGL;

namespace Esthar.Data
{
    public sealed class TimTexture2DReader
    {
        public static GLTexture Read(TimHeader header, TimImageHeader imageHeader, Stream input)
        {
            TimTexture2DReader converter = new TimTexture2DReader(header, imageHeader, input);
            return converter.Read();
        }

        private readonly TimHeader _header;
        private readonly TimImageHeader _imageHeader;
        private readonly Stream _input;

        public TimTexture2DReader(TimHeader header, TimImageHeader imageHeader, Stream input)
        {
            _header = Exceptions.CheckArgumentNull(header, "header");
            _imageHeader = Exceptions.CheckArgumentNull(imageHeader, "imageHeader");
            _input = Exceptions.CheckReadableStream(input, "input");
        }

        private GLTexture Read()
        {
            switch (_header.BytesPerPixel)
            {
                case 0:
                case 1:
                    return GLTextureFactory.FromStream(_input, _imageHeader.Width, _imageHeader.Height, PixelFormat.Format8bppIndexed);
                case 2:
                    return GLTextureFactory.FromStream(_input, _imageHeader.Width, _imageHeader.Height, PixelFormat.Format16bppArgb1555);
                case 3:
                    return GLTextureFactory.FromStream(_input, _imageHeader.Width, _imageHeader.Height, PixelFormat.Format24bppRgb);
                case 4:
                    return GLTextureFactory.FromStream(_input, _imageHeader.Width, _imageHeader.Height, PixelFormat.Format32bppArgb);
                default:
                    throw new NotSupportedException();
            }
        }
    }
}