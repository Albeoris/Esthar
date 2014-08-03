using System;
using System.IO;
using Esthar.Core;
using Esthar.OpenGL;

namespace Esthar.Data.Transform
{
    public static class GameImageReader
    {
        public static GameImage FromTex(TexFileReader reader)
        {
            const short x = 0;
            const short y = 0;

            using (DisposableStack insurance = new DisposableStack(2))
            {
                GLTexture layer = insurance.Add(reader.ReadImage());
                GLTexture palettes = insurance.Add(GLTextureFactory.FromBitmapPalettes(reader.Palettes));
                GameImage result = new GameImage(x, y, layer, palettes);
                insurance.Clear();
                return result;
            }
        }

        public static GameImage FromTim(TimFileReader reader)
        {
            using (DisposableStack insurance = new DisposableStack(3))
            {
                TimImage timImage = insurance.Add(reader.ReadImage());
                short x = timImage.ImageHeader.X;
                short y = timImage.ImageHeader.Y;

                GLTexture layer = timImage.Layer;
                GLTexture palettes = insurance.Add(GLTextureFactory.FromBitmapPalettes(reader.Palettes == null ? null : reader.Palettes.Palettes));

                GameImage result = new GameImage(x, y, layer, palettes);
                insurance.Clear();
                return result;
            }
        }

        public static GameImage FromDirectory(string dirPath)
        {
            Exceptions.CheckArgumentNullOrEmprty(dirPath, "dirPath");
            Exceptions.CheckDirectoryNotFoundException(dirPath);

            string generalName = Path.GetFileNameWithoutExtension(dirPath);
            GLTexture palettes = ReadPalettes(dirPath, generalName);
            GLTexture layer = ReadLayer(dirPath, generalName);

            return new GameImage(0, 0, layer, palettes);
        }

        private static GLTexture ReadPalettes(string dirPath, string generalName)
        {
            return GLTextureFactory.FromActPalleteFiles(dirPath, generalName);
        }

        private static GLTexture ReadLayer(string dirPath, string generalName)
        {
            string[] files = Directory.GetFiles(dirPath, generalName + ".???");
            if (files.Length == 0)
                return null;

            foreach (string file in files)
            {
                switch ((Path.GetExtension(file) ?? String.Empty).ToLower())
                {
                    case ".png":
                    case ".gif":
                        return GLTextureFactory.FromImageFile(file);
                }
            }

            return null;
        }
    }
}