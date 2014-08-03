using System.IO;
using System.Windows.Media;
using Esthar.Core;
using Esthar.OpenGL;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Esthar.Data
{
    public sealed class TexTexture2DReader
    {
        private readonly TexHeader _header;
        private readonly Stream _input;

        public TexTexture2DReader(TexHeader header, Stream input)
        {
            _header = Exceptions.CheckArgumentNull(header, "header");
            _input = Exceptions.CheckReadableStream(input, "input");
        }

        public GLTexture Read()
        {
            int size = _header.ImageWidth * _header.ImageHeight * _header.BytesPerPixel;
            PixelFormatDescriptor format = GetColorFormat();
            if (format != null)
                return ReadTexture(_input, format);

            using (SafeHGlobalHandle pixels = _input.ReadBuff(size))
            {
                TexPixelFormatsConverter converter = new TexPixelFormatsConverter(_header, pixels, PixelFormats.Bgra32);
                using (SafeHGlobalHandle newPixels = converter.Convert())
                using (UnmanagedMemoryStream input = newPixels.OpenStream(FileAccess.Read))
                    return ReadTexture(input, PixelFormat.Format32bppArgb);
            }
        }

        private GLTexture ReadTexture(Stream input, PixelFormat format)
        {
            return GLTextureFactory.FromStream(input, _header.ImageWidth, _header.ImageHeight, format);
        }

        private PixelFormatDescriptor GetColorFormat()
        {
            //switch (_header.BytesPerPixel)
            //{
            //    //_header.NumberOfBlueBitsConst, _header.NumberOfGreenBitsConst, _header.NumberOfRedBitsConst
            //}
            return null;
        }
    }
}