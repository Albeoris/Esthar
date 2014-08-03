using System.Windows;
using System.Windows.Media.Imaging;

namespace Esthar.Core
{
    public static class BitmapSourceExm
    {
        public static SafeHGlobalHandle GetPixels(this BitmapSource self, out Int32Rect rect)
        {
            int size = self.PixelWidth * self.PixelHeight * self.Format.BitsPerPixel / 8;
            rect = new Int32Rect(0, 0, self.PixelWidth, self.PixelHeight);
            SafeHGlobalHandle buffer = new SafeHGlobalHandle(size);
            try
            {
                self.CopyPixels(rect, buffer.DangerousGetHandle(), size, size / self.PixelHeight);
                return buffer;
            }
            catch
            {
                buffer.SafeDispose();
                throw;
            }
        }
    }
}