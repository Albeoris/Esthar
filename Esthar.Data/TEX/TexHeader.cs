using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Esthar.Core;
using Esthar.OpenGL;
using OpenTK.Graphics.OpenGL;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public sealed class TexHeader
    {
        public TexVersion Version; // Version, must be 1, or FF7 won't load the file
        public int Unknown1;
        public int ColorKeyFlag;
        public int Unknown2;
        public int Unknown3;
        public int MinBitsPerColor; // (D3D driver uses these to determine which texture format to convert to on load)
        public int MaxBitsPerColor; // (D3D driver uses these to determine which texture format to convert to on load)
        public int MinAlphaBit;
        public int MaxAlphaBit;
        public int MinBitsPerPixel;
        public int MaxBitsPerPixel;
        public int Unknown4;
        public int NumberOfPalettes;
        public int NumberOfColorsPerPalette1;
        public int BitDepth;
        public int ImageWidth;
        public int ImageHeight;
        public int PitchOrBytesPerRow; // usually ignored and assumed to be bytes per pixel * width
        public int Unknown5;
        public int PaletteFlag; // Наличие палиты
        public int BitsPerIndex; // Всегда 0 для изображений без палитры
        public int IndexedTo8BitFlag; // Никогда не используется в FF7
        public int PaletteSize;
        public int NumberOfColorsPerPalette2; // again, may be 0 sometimes, the other value will be used anyway
        public int RuntimeData; // игнорируется
        public int BitsPerPixel;
        public int BytesPerPixel; // always use this to determine how much data to read, if this is 1 you read 1 byte per pixel, regardless of bit depth.

        // Pixel format (all 0 for paletted images).
        public int NumberOfRedBits;
        public int NumberOfGreenBits;
        public int NumberOfBlueBits;
        public int NumberOfAlphaBits;
        public int RedBitmask;
        public int GreenBitmask;
        public int BlueBitmask;
        public int AlphaBitmask;
        public int RedShift;
        public int GreenShift;
        public int BlueShift;
        public int AlphaShift;
        public int NumberOfRedBitsConst; // Not sure what the point of these fields is, they're always ignored anyway
        public int NumberOfGreenBitsConst;
        public int NumberOfBlueBitsConst;
        public int NumberOfAlphaBitsConst;
        public int RedMax;
        public int GreenMax;
        public int BlueMax;
        public int AlphaMax;
        // End of pixel format

        public int ColorKeyArrayFlag; // this indicates the presence of a color key array
        public int RuntimeData2;
        public int ReferenceAlpha; // more on this later
        public int RuntimeData3;
        public int Unknown;
        public int PaletteIndex; // Rutime data
        public int RuntimeData4;
        public int RuntimeData5;
        public int Unknown6;
        public int Unknown7;
        public int Unknown8; // 224 => половина высоты экрана
        public int Unknown9;
        public int Unknown10;

        public static TexHeader Read(Stream input)
        {
            TexHeader result = input.ReadStruct<TexHeader>();
            result.Validate();

            return result;
        }

        private void Validate()
        {
            if (Version != TexVersion.FF8)
                throw new NotSupportedException(String.Format("Версия '{0}' TEX-файла не поддерживается текущей версией программы.", Version));

            if (PaletteFlag == 0)
            {
                if (NumberOfPalettes != 0)
                    throw new Exception(String.Format("PaletteFlag: {0} != NumberOfPalettes: {1}", PaletteFlag, NumberOfPalettes));
            }
            else if (NumberOfPalettes == 0)
            {
                throw new Exception(String.Format("PaletteFlag: {0} != NumberOfPalettes: {1}", PaletteFlag, NumberOfPalettes));
            }
            else if (PaletteSize == 0)
            {
                throw new Exception(String.Format("PaletteFlag: {0}, PaletteSize: {1}", PaletteFlag, PaletteSize));
            }

            if (NumberOfColorsPerPalette1 != NumberOfColorsPerPalette2)
                throw new Exception(String.Format("NumberOfColorsPerPalette1: {0} != NumberOfColorsPerPalette2: {1}", NumberOfColorsPerPalette1, NumberOfColorsPerPalette2));

            if (BitsPerPixel != BytesPerPixel * 8)
                throw new Exception(String.Format("BitsPerPixel: {0} != BytesPerPixel * 8: {1}", BitsPerPixel, BytesPerPixel));
        }

        public BitmapPalette[] TryReadPalette(Stream input)
        {
            if (PaletteFlag == 0)
                return null;

            byte[] colorKeys = TryReadColorKeys(input);

            var result = new List<BitmapPalette>(NumberOfPalettes);
            Color[] colors = new Color[NumberOfColorsPerPalette1];

            while (result.Count < result.Capacity)
            {

                using (BinaryReader br = new BinaryReader(input, Encoding.ASCII, true))
                    for (int i = 0; i < colors.Length; i++)
                    {
                        unsafe
                        {
                            int value = br.ReadInt32();
                            byte* b = (byte*)&value;
                            if (b[3] == 254)
                                b[3] = (byte)ReferenceAlpha;
                            colors[i] = Color.FromArgb(b[3], b[2], b[1], b[0]);
                        }
                    }

                if (ColorKeyFlag != 0)
                {
                    if (colorKeys != null)
                        colors[colorKeys[result.Count] - 1].A = 0;
                    else
                        colors[ColorKeyFlag - 1].A = 0;
                }

                result.Add(new BitmapPalette(colors));
            }

            return result.ToArray();
        }

        public byte[] TryReadColorKeys(Stream input)
        {
            if (ColorKeyArrayFlag == 0)
                return null;

            long position = input.Position;
            input.Position = input.Length - 1 - NumberOfPalettes;

            byte[] result = new byte[NumberOfPalettes];
            input.Read(result, 0, NumberOfPalettes);

            input.Position = position;
            return result;
        }

        public GLTexture TryReadData(Stream input)
        {
            return GLTextureFactory.FromStream(input, ImageWidth, ImageHeight, PixelFormat.Format8bppIndexed);
        }

        public static TexHeader Create(BitmapFrame frame, BitmapPalette[] palettes)
        {
            TexHeader result = new TexHeader
            {
                Version = TexVersion.FF8,
                ImageWidth = frame.PixelWidth,
                ImageHeight = frame.PixelHeight,
                ColorKeyArrayFlag = 0,

                MinAlphaBit = 4,
                MaxAlphaBit = 8,
                MinBitsPerColor = 4,
                MaxBitsPerColor = 8,
                MinBitsPerPixel = 8,
                MaxBitsPerPixel = 32,
                ReferenceAlpha = 255
            };

            if (frame.Format == PixelFormats.Indexed8)
            {
                if (palettes == null || palettes.Length == 0)
                    throw new ArgumentNullException("palettes");

                FillIndexedProperties(result, palettes);
            }
            else if (frame.Format == PixelFormats.Bgra32)
            {
                FillBgraProperties(result);
            }
            else
            {
                throw new NotSupportedException(String.Format("Неподдерживаемый формат изображения: {0}, поддерживаются только {1} и {2}.", frame.Format, PixelFormats.Bgra32, PixelFormats.Indexed8));
            }

            return result;
        }

        public static TexHeader Create(GLTexture texture, GLTexture palettes, bool b)
        {
            TexHeader result = new TexHeader
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
                ReferenceAlpha = 255
            };

            if (texture.PixelFormat.GLPixelInternalFormat == PixelInternalFormat.R8)
            {
                if (palettes == null)
                    throw new ArgumentNullException("palettes");

                FillIndexedProperties(result, palettes);
            }
            else
            {
                throw new NotSupportedException(String.Format("Неподдерживаемый формат изображения: {0}.", texture.PixelFormat.GLPixelInternalFormat));
            }

            return result;
        }

        private static void FillIndexedProperties(TexHeader header, GLTexture palettes)
        {
            using (GLService.AcquireContext())
            {
                int frameBuffer = GL.Ext.GenFramebuffer();
                GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, frameBuffer);
                GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, palettes.Id, 0);
                
                byte[] pixels = new byte[4];
                GL.ReadPixels(0, 0, 1, 1, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, pixels);
                GL.Ext.DeleteFramebuffer(frameBuffer);

                if (pixels[3] == 0)
                    header.ColorKeyFlag = 1;
            }

            header.BytesPerPixel = 1;
            header.BitDepth = 8;
            header.BitsPerIndex = 8;
            header.BitsPerPixel = 8;

            header.PaletteFlag = 1;
            header.NumberOfPalettes = palettes.Height;
            header.NumberOfColorsPerPalette1 = palettes.Width;
            header.NumberOfColorsPerPalette2 = palettes.Width;
            header.PaletteSize = palettes.Width * palettes.Height;

            header.Unknown7 = 288;
            header.Unknown8 = 224;
        }

        private static void FillIndexedProperties(TexHeader header, BitmapPalette[] palettes)
        {
            BitmapPalette palette = palettes[0];
            Color color = palette.Colors[0];

            if (color.A == 0)
                header.ColorKeyFlag = 1;

            header.BytesPerPixel = 1;
            header.BitDepth = 8;
            header.BitsPerIndex = 8;
            header.BitsPerPixel = 8;

            header.PaletteFlag = 1;
            header.NumberOfPalettes = palettes.Length;
            header.NumberOfColorsPerPalette1 = palette.Colors.Count;
            header.NumberOfColorsPerPalette2 = palette.Colors.Count;
            header.PaletteSize = palettes.Length * palette.Colors.Count;

            header.Unknown7 = 288;
            header.Unknown8 = 224;
        }

        private static void FillBgraProperties(TexHeader header)
        {
            throw new NotImplementedException();
        }
    }
}