using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows;
using Esthar.Core;
using OpenTK.Graphics.OpenGL;

namespace Esthar.OpenGL
{
    public sealed class GLLocationPageRenderer
    {
        private const float BackgroundZ = 0.0f;
        private const float WindowZ = 0.1f;

        private const int TextOffset = 8;
        private const int CharSize = 24;
        private const int ScreenWidth = 640;
        public const int ScreenHeight = 448;

        private readonly GLLocationPagesRendererContext _context;
        private readonly char[] _text;

        private int _y, _ox, _oy, _line, _charIndex, _charsLeft;
        private bool _calcOnly;

        public GLLocationPageRenderer(int index, GLLocationPagesRendererContext context, string text)
        {
            _y = index * ScreenHeight;
            _context = context;
            _text = text == null ? null : text.ToCharArray();
        }
        
        public void DrawBackground()
        {
            if (_context.Background != null)
            {
                GL.Enable(EnableCap.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, _context.Background.Id);
                GL.Begin(PrimitiveType.Quads);
                GL.Color4(Color.Transparent);

                float w = _context.Background.Width;
                float h = _context.Background.Height;
                float k = ScreenWidth / w;
                float kh = ScreenHeight / h;
                k = k < kh ? kh : k;
                w *= k;
                h *= k;
                
                GL.TexCoord2(0, 0); GL.Vertex3(0, _y, BackgroundZ);
                GL.TexCoord2(1, 0); GL.Vertex3(w, _y, BackgroundZ);
                GL.TexCoord2(1, 1); GL.Vertex3(w, _y + h, BackgroundZ);
                GL.TexCoord2(0, 1); GL.Vertex3(0, _y + h, BackgroundZ);

                GL.End();
                GL.Disable(EnableCap.Texture2D);
            }
        }

        public void DrawText()
        {
            if (_text == null)
                return;

            _calcOnly = false;
            DrawTextsInternal();
        }

        public void CalcWindowSize()
        {
            _calcOnly = true;
            DrawTextsInternal();
        }

        private void DrawTextsInternal()
        {
            _ox = 0;
            _oy = TextOffset - CharSize;
            _context.SymbolRenderer.Palette = FF8TextTagColor.White;
            _line = -1;
            _charIndex = 0;
            _charsLeft = _text.Length;

            NewLine();
            while (_charsLeft > 0)
            {
                CheckNewLines();
                if (_oy >= ScreenHeight)
                    break;

                FF8TextTag tag = FF8TextTag.TryRead(_text, ref _charIndex, ref _charsLeft);
                if (tag != null)
                {
                    DrawTag(tag);
                }
                else if (FF8TextComment.TryRead(_text, ref _charIndex, ref _charsLeft) == null)
                {
                    DrawChar(_text[_charIndex++]);
                    _charsLeft--;
                }

                if (_ox >= ScreenWidth)
                    SkipLine();
            }

            _oy += CharSize;
            if (_oy > _context.MaxHeight)
                _context.MaxHeight = _oy;
        }

        private void SkipLine()
        {
            while (_charsLeft > 0 && _text[_charIndex] != '\r' && _text[_charIndex] != '\n')
            {
                _charIndex++;
                _charsLeft--;
            }
        }

        private void CheckNewLines()
        {
            while (_charsLeft > 0)
            {
                if (_text[_charIndex] == '\r')
                {
                    _charIndex++;
                    _charsLeft--;

                    if (_charsLeft < 1)
                        break;
                }

                if (_text[_charIndex] == '\n')
                {
                    _charIndex++;
                    _charsLeft--;

                    NewLine();
                    continue;
                }
                break;
            }
        }

        private void NewLine()
        {
            _oy += CharSize;
            _ox = TextOffset;
            _line++;

            if (_line >= _context.FirstAnswer && _line <= _context.LastAnswer)
            {
                int x = _context.GetWindowCornerX() + _ox;
                int y = _y + _oy + _context.GetWindowCornerY();

                if (!_calcOnly)
                {
                    if (_context.DefaultAnswer == _line)
                        _context.CursorImage.Draw(x, y, GLFontSymbolRenderer.TextZ);
                    else if (_context.CancelAnswer == _line)
                        _context.DisabledCursorImage.Draw(x, y, GLFontSymbolRenderer.TextZ);
                }
                _ox += CharSize;
            }
        }

        private static readonly Color LeftSideWindowGradientColor = Color.FromArgb(66, 47, 42);
        private static readonly Color RightSideWindowGradientColor = Color.FromArgb(93, 93, 93);

        public void DrawWindow()
        {
            float[] coords = new float[12];
            int x = _context.GetWindowCornerX();
            int y = _context.GetWindowCornerY() + _y;
            int w = _context.MaxWidth + 8;
            int h = _context.MaxHeight + 8;

            coords[0] = x; // Bottom-left
            coords[1] = y + h; // Bottom-left
            coords[2] = WindowZ; // Bottom-left
            coords[3] = x; // Top-left
            coords[4] = y; // Top-left
            coords[5] = WindowZ; // Top-left
            coords[6] = x + w; // Top-right
            coords[7] = y; // Top-right
            coords[8] = WindowZ; // Top-right
            coords[9] = x + w; // Bottom-right
            coords[10] = y + h; // Bottom-right
            coords[11] = WindowZ; // Bottom-right

            unsafe
            {
                fixed (float* ptr = &coords[0])
                {
                    // Background
                    GL.Begin(PrimitiveType.Quads);
                    {
                        GL.Color3(LeftSideWindowGradientColor);
                        GL.Vertex3(ptr + 0 * 3);
                        GL.Vertex3(ptr + 1 * 3);

                        GL.Color3(RightSideWindowGradientColor);
                        GL.Vertex3(ptr + 2 * 3);
                        GL.Vertex3(ptr + 3 * 3);
                    }
                    GL.End();

                    // Border
                    coords[2] += WindowZ;
                    coords[5] += WindowZ;
                    coords[8] += WindowZ;
                    coords[11] += WindowZ;
                    GL.Begin(PrimitiveType.LineLoop);
                    {
                        GL.Color3(Color.WhiteSmoke);
                        for (int i = 0; i < 4; i++)
                            GL.Vertex3(ptr + i * 3);
                    }
                    GL.End();
                }
            }
        }

        private void DrawString(string chars)
        {
            foreach (char ch in chars)
                DrawChar(ch);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawChar(char ch)
        {
            _ox += _context.SymbolRenderer.DrawChar(ch, _context.GetWindowCornerX() + _ox, _y + _oy + _context.GetWindowCornerY(), _calcOnly);
            if (_ox > _context.MaxWidth)
                _context.MaxWidth = _ox;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawTag(FF8TextTag tag)
        {
            switch (tag.Code)
            {
                case FF8TextTagCode.Color:
                    _context.SymbolRenderer.Palette = (FF8TextTagColor)tag.Param;
                    break;
                default:
                    DrawChar('~');
                    DrawString(tag.Param == null ? tag.Code.ToString() : tag.Param.ToString());
                    DrawChar('~');
                    break;
            }
        }
    }
}