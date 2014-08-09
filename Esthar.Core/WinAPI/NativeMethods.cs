using System;
using System.Runtime.InteropServices;

namespace Esthar.Core
{
    public static class NativeMethods
    {
        [DllImport("User32.dll", SetLastError = true)]
        public static extern int SetWindowRgn(IntPtr handle, IntPtr region, bool redraw);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject(IntPtr handle);

        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string path, uint attributes, ref ShellFileSystemInfo info, int fileInfoSize, SHGetFileInfoFlags flags);
    }
}