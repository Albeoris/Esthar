using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Esthar.OpenGL;

namespace Esthar
{
    public static class ResourcesAccessor
    {
        public static GLTexture CursorImage
        {
            get { return LazyCursorImage.Value; }
        }

        public static GLTexture DisabledCursorImage
        {
            get { return LazyDisabledCursorImage.Value; }
        }

        private const string CursorPath = "Esthar.Resources.Cursor.png";
        private const string DisabledCursor = "Esthar.Resources.DisabledCursor.png";

        private static readonly Lazy<GLTexture> LazyCursorImage = new Lazy<GLTexture>(LoadCursorImage, true);
        private static readonly Lazy<GLTexture> LazyDisabledCursorImage = new Lazy<GLTexture>(LoadDisabledCursorImage, true);

        public static GLTexture LoadCursorImage()
        {
            using (Stream input = Assembly.GetExecutingAssembly().GetManifestResourceStream(CursorPath))
            using (Bitmap bitmap = new Bitmap(input))
            {
                return GLTextureFactory.FromBitmap(bitmap, bitmap.Width, bitmap.Height, bitmap.PixelFormat);
            }
        }

        public static GLTexture LoadDisabledCursorImage()
        {
            using (Stream input = Assembly.GetExecutingAssembly().GetManifestResourceStream(DisabledCursor))
            using (Bitmap bitmap = new Bitmap(input))
            {
                return GLTextureFactory.FromBitmap(bitmap, bitmap.Width, bitmap.Height, bitmap.PixelFormat);
            }
        }
    }
}