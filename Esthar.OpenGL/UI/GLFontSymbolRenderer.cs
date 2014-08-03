using System.Drawing;
using Esthar.Core;
using OpenTK.Graphics.OpenGL;

namespace Esthar.OpenGL
{
    public sealed class GLFontSymbolRenderer
    {
        public const float TextZ = 0.2f;

        private readonly GLTexture _fontTexture;
        private readonly GLTexture _fontPalletes;
        private readonly byte[] _fontWidths;

        private readonly float _fontTextureWidth, _fontTextureHeight;
        private readonly int _colCount;

        private int _paletteIndex;

        public FF8TextTagColor Palette
        {
            set { _paletteIndex = (value - FF8TextTagColor.Disabled) % (FF8TextTagColor.White - FF8TextTagColor.Disabled + 1); }
        }

        public GLFontSymbolRenderer(GLTexture fontTexture, GLTexture fontPalletes, byte[] fontWidths)
        {
            _fontTexture = fontTexture;
            _fontPalletes = fontPalletes;
            _fontWidths = fontWidths;

            _fontTextureWidth = _fontTexture.Width;
            _fontTextureHeight = _fontTexture.Height;
            _colCount = _fontTexture.Width / 24;
        }

        public int DrawChar(char ch, int ox, int oy, bool calcOnly)
        {
            byte? rawIndex = Options.Codepage.TryGetByte(ch);
            if (rawIndex == null)
                return DrawInvalidChar(ox, oy, calcOnly);

            int index = rawIndex.Value - 32;

            int x = index % _colCount * 24;
            int y = index / _colCount * 24;
            int w = _fontWidths[index] * 2;
            if (calcOnly)
                return w;
            const int h = 23;

            float tx = x / _fontTextureWidth;
            float ty = y / _fontTextureHeight;
            float tw = w / _fontTextureWidth;
            float th = h / _fontTextureHeight;

            x = ox;
            y = oy;

            using (GLShaders.PalettedTexture.Use(_fontTexture, _fontPalletes, _paletteIndex))
            {
                GL.Begin(PrimitiveType.Quads);

                GL.TexCoord2(tx, ty); GL.Vertex3(x, y, TextZ);
                GL.TexCoord2(tx + tw, ty); GL.Vertex3(x + w, y, TextZ);
                GL.TexCoord2(tx + tw, ty + th); GL.Vertex3(x + w, y + h, TextZ);
                GL.TexCoord2(tx, ty + th); GL.Vertex3(x, y + h, TextZ);

                GL.End();
            }

            return w;
        }

        private int DrawInvalidChar(int x, int y, bool calcOnly)
        {
            const int w = 8;
            const int h = 24;

            if (calcOnly)
                return w;

            float[] coords =
            {
                x, y + h, TextZ, // Bottom-left
                x, y, TextZ, // Top-left
                x + w, y, TextZ, // Top-right
                x + w, y + h, TextZ // Bottom-right
            };

            unsafe
            {
                fixed (float* ptr = &coords[0])
                {
                    //GL.Disable(EnableCap.Texture2D);

                    GL.Begin(PrimitiveType.Quads);
                    {
                        GL.Color3(Color.DarkRed);
                        for (int i = 0; i < 4; i++)
                            GL.Vertex3(ptr + i * 3);
                    }
                    GL.End();
                }
            }

            return w;
        }
    }
}