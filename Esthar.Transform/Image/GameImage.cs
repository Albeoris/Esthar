using System;
using System.Windows.Media.Imaging;
using Esthar.Core;
using Esthar.OpenGL;

namespace Esthar.Data.Transform
{
    public sealed class GameImage : IDisposable
    {
        public readonly int X, Y;
        public readonly GLTexture Layer;
        public readonly GLTexture Palettes;

        public GameImage(int x, int y, GLTexture layer, GLTexture palettes)
        {
            Layer = Exceptions.CheckArgumentNull(layer, "layer");
            Palettes = palettes;

            X = x;
            Y = y;
        }

        public void Dispose()
        {
            Layer.NullSafeDispose();
            Palettes.NullSafeDispose();
        }

        public int Width
        {
            get { return Layer.Width; }
        }

        public int Height
        {
            get { return Layer.Height; }
        }
    }
}