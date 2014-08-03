using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Esthar.Core;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Esthar.OpenGL
{
    public sealed class ActPalleteFilesGLTextureReader : GLTextureReader
    {
        private readonly string _dirPath;
        private readonly string _generalName;

        public ActPalleteFilesGLTextureReader(string dirPath, string generalName)
        {
            _dirPath = dirPath;
            _generalName = generalName;
        }

        public override async Task<GLTexture> ReadTextureAsync(CancellationToken cancelationToken)
        {
            if (cancelationToken.IsCancellationRequested)
                return RaiseTextureReaded(null);

            string[] files = Directory.GetFiles(_dirPath, String.Format("{0}_*.act", _generalName));
            if (files.Length == 0 || cancelationToken.IsCancellationRequested)
                return RaiseTextureReaded(null);

            using (Bitmap bitmap = new Bitmap(256, files.Length, PixelFormat.Format32bppArgb))
            {
                BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);

                int size = bitmapData.Stride * bitmapData.Height;
                using (UnmanagedMemoryStream output = bitmapData.Scan0.OpenStream(size, FileAccess.Write))
                {
                    foreach (string file in files)
                    {
                        if (cancelationToken.IsCancellationRequested)
                            return RaiseTextureReaded(null);

                        Read(file, output);
                    }
                }

                bitmap.UnlockBits(bitmapData);

                BitmapGLTextureReader bitmapReader = new BitmapGLTextureReader(bitmap, bitmap.Width, bitmap.Height, bitmap.PixelFormat);
                return RaiseTextureReaded(await bitmapReader.ReadTextureAsync(cancelationToken));
            }
        }

        private static void Read(string fullPath, Stream pixelsOutput)
        {
            using (FileStream input = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                Read(input, pixelsOutput);
        }

        private static void Read(FileStream input, Stream pixelsOutput)
        {
            byte[] buff = new byte[4];
            buff[3] = 0xFF;

            for (int i = 0; i < 256; i++)
            {
                input.EnsureRead(buff, 0, 3);
                //buff.Swap(0, 2);
                pixelsOutput.Write(buff, 0, 4);
            }

            if (!input.IsEndOfStream())
            {
                input.EnsureRead(buff, 0, 4);
                if (BitConverter.ToInt32(buff, 0) == 1)
                {
                    pixelsOutput.Seek(-256 * 4 + 3, SeekOrigin.Current);
                    pixelsOutput.WriteByte(0);
                    pixelsOutput.Seek(255 * 4, SeekOrigin.Current);
                }
            }
        }
    }
}