using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using Esthar.Core;

namespace Esthar.UI
{
    public static class ShellHelper
    {
        public static BitmapSource ExtractAssociatedIcon(string path)
        {
            ShellFileSystemInfo info = new ShellFileSystemInfo();
            NativeMethods.SHGetFileInfo(path, 0, ref info, Marshal.SizeOf(info), SHGetFileInfoFlags.Icon | SHGetFileInfoFlags.LargeIcon);
            using (Icon icon = Icon.FromHandle(info.IconHandle))
            using (Bitmap bitmap = icon.ToBitmap())
                return BitmapConverter.ToBitmapSource(bitmap);
        }
    }
}