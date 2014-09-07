using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Esthar.Core;
using OpenTK.Graphics.OpenGL;

namespace Esthar.OpenGL
{
    public sealed class ActPalleteFilesGLTextureWriter : IGLTextureWriter
    {
        private readonly GLTexture _texture;
        private readonly string _generalPath;

        public ActPalleteFilesGLTextureWriter(GLTexture texture, string generalPath)
        {
            if (texture.PixelFormat.DrawingFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                throw new ArgumentException("texture");

            _texture = Exceptions.CheckArgumentNull(texture, "texture");
            _generalPath = Exceptions.CheckArgumentNullOrEmprty(generalPath, "generalPath");
        }

        public void Write()
        {
            string prefix = Path.ChangeExtension(_generalPath, null) + "_";

            using (Bitmap bmp = new Bitmap(_texture.Width, _texture.Height, _texture.PixelFormat))
            {
                BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, _texture.PixelFormat);
                using (GLService.AcquireContext())
                {
                    GL.BindTexture(TextureTarget.Texture2D, _texture.Id);
                    GL.GetTexImage(TextureTarget.Texture2D, 0, _texture.PixelFormat, _texture.PixelFormat, data.Scan0);
                }
                bmp.UnlockBits(data);

                using (UnmanagedMemoryStream input = data.Scan0.OpenStream(data.Stride * data.Height, FileAccess.Read))
                {
                    for (int i = 0; i < _texture.Height; i++)
                    {
                        string filePath = prefix + i.ToString("D2") + ".act";
                        Write(input, filePath);
                    }
                }
            }
        }

        private static void Write(Stream paletteStream, string filePath)
        {
            Exceptions.CheckArgumentNullOrEmprty(filePath, "filePath");

            using (Stream output = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                Write(paletteStream, output);
        }

        private static void Write(Stream paletteStream, Stream output)
        {
            Exceptions.CheckArgumentNull(paletteStream, "palette");
            Exceptions.CheckArgumentNull(output, "output");

            byte[] buff = new byte[4];
            paletteStream.EnsureRead(buff, 0, 4);
            buff.Swap(0, 2);
            output.Write(buff, 0, 3);
            bool hasTransperentColor = buff[3] == 0;

            for (int i = 0; i < 255; i++)
            {
                paletteStream.Read(buff, 0, 4);
                buff.Swap(0, 2);
                output.Write(buff, 0, 3);
            }

            if (hasTransperentColor)
            {
                unsafe
                {
                    fixed (byte* ptr = buff)
                        *(int*)ptr = 1;
                }
            }

            output.Write(buff, 0, 4);
        }
    }
}