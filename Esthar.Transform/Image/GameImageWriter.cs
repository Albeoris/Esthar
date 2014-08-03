using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using Esthar.Core;
using Esthar.OpenGL;

namespace Esthar.Data.Transform
{
    public static class GameImageWriter
    {
        public static void ToDirectory(GameImage image, string dirPath)
        {
            Exceptions.CheckArgumentNull(image, "image");
            Exceptions.CheckArgumentNullOrEmprty(dirPath, "dirPath");

            DirectoryInfo dir = new DirectoryInfo(dirPath);
            if (!dir.Exists)
            {
                dir.Create();
            }
            else
            {
                IEnumerable<FileInfo> files = dir
                    .GetFilesByExtensions(".png", ".act", ".gif")
                    .Where(file => file.Name.StartsWith(dir.Name, StringComparison.InvariantCultureIgnoreCase));

                foreach (FileInfo file in files)
                    file.Delete();
            }

            string generalPath = Path.Combine(dirPath, Path.GetFileNameWithoutExtension(dirPath));

            WritePalettes(image, generalPath);
            WriteLayer(image, generalPath);
        }

        private static void WritePalettes(GameImage image, string generalPath)
        {
            if (image.Palettes != null)
                image.Palettes.ToAct(generalPath);
        }

        private static void WriteLayer(GameImage image, string generalPath)
        {
            image.Layer.ToImageFile(generalPath);
        }
    }
}