using System;
using System.IO;
using System.Windows.Media;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class TexPixelFormatsConverter
    {
        private readonly TexHeader _header;
        private readonly SafeHGlobalHandle _source;
        private readonly PixelFormat _format;
        private readonly byte[] _buff;

        public TexPixelFormatsConverter(TexHeader header, SafeHGlobalHandle data, PixelFormat format)
        {
            if (header == null)
                throw new ArgumentException("header");
            if (data == null || data.IsInvalid)
                throw new ArgumentException("data");
            if (format != PixelFormats.Bgra32)
                throw new ArgumentOutOfRangeException("format");

            _header = header;
            _source = data;
            _format = format;

            _buff = new byte[header.BytesPerPixel];
        }

        public SafeHGlobalHandle Convert()
        {
            if (_format == PixelFormats.Bgra32)
                return ToBgra();
            
            return null;
        }

        private SafeHGlobalHandle ToBgra()
        {
            SafeHGlobalHandle result = new SafeHGlobalHandle(_header.ImageWidth * _header.ImageHeight * 4);
            try
            {
                ToBgra(result);
            }
            catch
            {
                result.Dispose();
                throw;
            }
            return result;
        }

        private void ToBgra(SafeHGlobalHandle result)
        {
            byte[] bgra = new byte[4];
            using (UnmanagedMemoryStream input = new UnmanagedMemoryStream(_source, (long)_source.ByteLength, (long)_source.ByteLength, FileAccess.Read))
            using (UnmanagedMemoryStream output = new UnmanagedMemoryStream(result, (long)result.ByteLength, (long)result.ByteLength, FileAccess.Write))
            {
                int pixelsCount = _header.ImageWidth * _header.ImageHeight;
                for (int i = 0; i < pixelsCount; i++)
                {
                    ReadPixel(input, bgra);
                    output.Write(bgra, 0, bgra.Length);
                }
            }
        }

        private void ReadPixel(Stream input, byte[] bgra)
        {
            if (input.Read(_buff, 0, _buff.Length) != _buff.Length)
                throw new Exception("Unexpected end of stream.");

            uint value = 0;
            for (int i = 0; i < _buff.Length; i++)
                value |= (uint)(_buff[i] << (8 * i));

            bgra[0] = (byte)(((value & _header.BlueBitmask) >> _header.BlueShift) << _header.NumberOfBlueBitsConst);
            bgra[1] = (byte)(((value & _header.GreenBitmask) >> _header.GreenShift) << _header.NumberOfGreenBitsConst);
            bgra[2] = (byte)(((value & _header.RedBitmask) >> _header.RedShift) << _header.NumberOfRedBitsConst);
            //bgra[3] = (byte)(((value & _header.AlphaBitmask) >> _header.AlphaShift) << _header.NumberOfAlphaBitsConst);
            bgra[3] = (byte)_header.ReferenceAlpha;
        }
    }
}