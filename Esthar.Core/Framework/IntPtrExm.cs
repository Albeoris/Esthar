using System;
using System.IO;

namespace Esthar.Core
{
    public static class IntPtrExm
    {
        public static UnmanagedMemoryStream OpenStream(this IntPtr self, long size, FileAccess access)
        {
            unsafe
            {
                return new UnmanagedMemoryStream((byte*)self.ToPointer(), size, size, access);
            }
        }
    }
}