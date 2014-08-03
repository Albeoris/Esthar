using System;
using System.IO;
using Esthar.Core;
using Esthar.OpenGL;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Esthar.Data
{
    public sealed class TexFileWriter : IDisposable
    {
        private readonly Stream _output;

        public TexFileWriter(Stream output)
        {
            _output = output;
        }

        public void Dispose()
        {
            _output.Dispose();
        }

        public void WriteImage(GLTexture texture, GLTexture palette)
        {
            if (palette == null)
                throw new NotImplementedException();

            WriteIndexedImage(texture, palette);
        }

        private void WriteIndexedImage(GLTexture texture, GLTexture palette)
        {
            byte[] palettesPixels = palette.GetManagedPixelsArray(PixelFormat.Format32bppArgb);
            TexHeader header = CreateHeader(texture, palette, palettesPixels[3] == 0);

            _output.WriteStruct(header);
            _output.Write(palettesPixels, 0, palettesPixels.Length);
            using (SafeHGlobalHandle pixels = texture.GetUnmanagedPixelsArray(PixelFormat.Format8bppIndexed))
            using (UnmanagedMemoryStream input = pixels.OpenStream(FileAccess.Read))
                input.CopyTo(_output);
        }

        private TexHeader CreateHeader(GLTexture texture, GLTexture palettes, bool hasKeyColor)
        {
            TexHeader header = new TexHeader
            {
                Version = TexVersion.FF8,
                ImageWidth = texture.Width,
                ImageHeight = texture.Height,
                ColorKeyArrayFlag = 0,

                MinAlphaBit = 4,
                MaxAlphaBit = 8,
                MinBitsPerColor = 4,
                MaxBitsPerColor = 8,
                MinBitsPerPixel = 8,
                MaxBitsPerPixel = 32,
                ReferenceAlpha = 255,

                // Палитра
                ColorKeyFlag = hasKeyColor ? 1 : 0,
                BytesPerPixel = 1,
                BitDepth = 8,
                BitsPerIndex = 8,
                BitsPerPixel = 8,
                PaletteFlag = 1,
                NumberOfPalettes = palettes.Height,
                NumberOfColorsPerPalette1 = palettes.Width,
                NumberOfColorsPerPalette2 = palettes.Width,
                PaletteSize = palettes.Width * palettes.Height,
                Unknown7 = 288,
                Unknown8 = 224
            };

            return header;
        }
    }
}