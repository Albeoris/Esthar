using System;
using System.Runtime.InteropServices;

namespace Esthar.Core
{
    public static class NativeMethods
    {
        [DllImport("User32.dll", SetLastError = true)]
        public static extern int SetWindowRgn(IntPtr handle, IntPtr region, bool redraw);
    }
}
