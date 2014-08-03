using System;

namespace Esthar.OpenGL
{
    public sealed class GLLocationPagesRendererContext
    {
        public int WindowX, WindowY;
        public int MaxWidth, MaxHeight;
        public int FirstAnswer, LastAnswer;
        public int DefaultAnswer, CancelAnswer;

        public GLTexture Background;
        public GLTexture CursorImage;
        public GLTexture DisabledCursorImage;
        public GLFontSymbolRenderer SymbolRenderer;

        public int GetWindowCornerX()
        {
            int x = 640 - (WindowX + MaxWidth / 2) - 16;
            x = x < 0 ? WindowX + x : WindowX;

            return Math.Max(8, x - MaxWidth / 2);
        }

        public int GetWindowCornerY()
        {
            int y = 448 - (WindowY + MaxHeight / 2) - 16;
            y = y < 0 ? WindowY + y : WindowY;

            return Math.Max(8, y - MaxHeight / 2);
        }
    }
}