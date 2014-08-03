using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Esthar.Core;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public sealed class TimHeader
    {
        private const int ValidMagicNumber = 0x00000010;

        public int MagicNumber;
        public int Value;

        public int BytesPerPixel
        {
            get { return Value & 3; }
        }

        public bool HasPalette
        {
            get { return (Value & 8) == 8; }
        }

        public int PaletteSize
        {
            get { return BytesPerPixel == 0 ? 16 : 256; }
        }

        public PixelFormat OutputPixelFormat
        {
            get
            {
                switch (BytesPerPixel)
                {
                    case 0:
                        return PixelFormats.Indexed4;
                    case 1:
                        return PixelFormats.Indexed8;
                    default:
                        return PixelFormats.Bgra32;
                }
            }
        }

        public static TimHeader Read(Stream input)
        {
            TimHeader result = input.ReadStruct<TimHeader>();
            result.Validate();

            return result;
        }

        private void Validate()
        {
            if (MagicNumber != ValidMagicNumber)
                throw new NotSupportedException(String.Format("Неверное магическое число в заголовке TIM-файла: '{0}'", MagicNumber));

            if (HasPalette && BytesPerPixel > 1)
                throw new Exception(String.Format("Для изображеня с индексированными числами BPP не может превышать 1: '{0}'", BytesPerPixel));
        }
    }
}