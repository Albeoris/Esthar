using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;
using Esthar.OpenGL;

namespace Esthar.Data
{
    public sealed class TexFileReader : GameFileReader
    {
        public TexHeader Header;
        public BitmapPalette[] Palettes;
        
        public TexFileReader(Stream input)
            : base(input)
        {
        }

        public override void Close()
        {
            IOStream.Seek(0, SeekOrigin.Begin);
            Header = null;
            Palettes = null;
        }

        public override void Open()
        {
            Close();

            Header = TexHeader.Read(IOStream);
            Palettes = Header.TryReadPalette(IOStream);
        }

        public GLTexture ReadImage()
        {
            if (Palettes.Length != 0)
                return ReadIndexedImage();
            return ReadFullColorImage();
        }

        private GLTexture ReadIndexedImage()
        {
            int size = Header.ImageWidth * Header.ImageHeight * Header.BytesPerPixel;
            return GLTextureFactory.FromStream(IOStream, Header.ImageWidth, Header.ImageHeight, PixelFormat.Format8bppIndexed);
        }

        private GLTexture ReadFullColorImage()
        {
            TexTexture2DReader reader = new TexTexture2DReader(Header, IOStream);
            return reader.Read();
        }
    }
}