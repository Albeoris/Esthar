using System;
using System.IO;
using System.Windows;
using Esthar.Core;
using Esthar.OpenGL;

namespace Esthar.Data.Transform
{
    public sealed class GameFont : IDisposable
    {
        #region Static

        private static readonly string FontDirectory = Path.Combine(Options.WorkingDirectory, "Font");

        private static readonly Lazy<GameFont> _hiResFont;

        static GameFont()
        {
            _hiResFont = new Lazy<GameFont>(LoadHiResFont);
        }

        private static GameFont LoadHiResFont()
        {
            GameFont font = GameFontReader.FromDirectory(FontDirectory);
            if (font == null)
            {
                font = GameFontReader.HiResFromGameData();
                GameFontWriter.ToDirectory(font, FontDirectory);
            }
            return font;
        }

        public static GameFont HiResFont
        {
            get { return _hiResFont.Value; }
        }

        #endregion

        public GameImage CharactersImage { get; private set; }
        public byte[] CharactersWidths { get; private set; }

        public GameFont(GameImage image, byte[] widths)
        {
            CharactersImage = Exceptions.CheckArgumentNull(image, "image");
            CharactersWidths = Exceptions.CheckArgumentNull(widths, "widths");
        }

        public void Dispose()
        {
            CharactersImage.SafeDispose();
        }

        public GLFontSymbolRenderer GetRenderer()
        {
            return new GLFontSymbolRenderer(CharactersImage.Layer, CharactersImage.Palettes, CharactersWidths);
        }

        public void UpdateTexture(GLTexture texture)
        {
            GLTexture old = CharactersImage.Layer;
            CharactersImage = new GameImage(CharactersImage.X, CharactersImage.Y, texture, CharactersImage.Palettes);
            old.SafeDispose();
        }

        public void UpdateWidths(byte[] charactersWidths)
        {
            CharactersWidths = charactersWidths;
        }

        public void SaveHiRes()
        {
            GameFontWriter.ToDirectory(this, FontDirectory);
        }

        public void ImportHiRes()
        {
            GameFontWriter.HiResToGameData(this);
        }
    }
}