using System;
using System.IO;
using Esthar.Core;
using Esthar.OpenGL;

namespace Esthar.Data
{
    public sealed class TimImage : IDisposable
    {
        public readonly TimImageHeader ImageHeader;
        public readonly GLTexture Layer;

        public TimImage(TimImageHeader imageHeader, GLTexture layer)
        {
            ImageHeader = Exceptions.CheckArgumentNull(imageHeader, "imageHeader");
            Layer = Exceptions.CheckArgumentNull(layer, "layer");
        }

        public void Dispose()
        {
            Layer.Dispose();
        }

        public static TimImage Read(TimHeader header, Stream input)
        {
            TimImageHeader imageHeader = TimImageHeader.Read(header, input);
            GLTexture layer = TimTexture2DReader.Read(header, imageHeader, input);

            //SafeHGlobalHandle pixels = new SafeHGlobalHandle(imageHeader.Width * imageHeader.Height * header.OutputPixelFormat.BitsPerPixel / 8);
            SafeHGlobalHandle pixels = new SafeHGlobalHandle(imageHeader.ContentSize);
            try
            {
                using (UnmanagedMemoryStream output = new UnmanagedMemoryStream(pixels, 0, (long)pixels.ByteLength, FileAccess.Write))
                    TimPixelFormatsConverter.Convert(header, imageHeader, input, output);
            }
            catch
            {
                pixels.Dispose();
                throw;
            }
            return new TimImage(imageHeader, layer);
        }
    }
}