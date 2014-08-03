using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Esthar.Core
{
    public sealed class SafeHGlobalHandle : SafeBuffer
    {
        public SafeHGlobalHandle(int size)
            : base(true)
        {
            if (size < 0)
                throw new ArgumentOutOfRangeException("size");

            SetHandle(Marshal.AllocHGlobal(size));
            Initialize((ulong)size);
        }

        protected override bool ReleaseHandle()
        {
            try
            {
                Marshal.FreeHGlobal(handle);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public int Length
        {
            get { return (int)ByteLength; }
        }

        public UnmanagedMemoryStream OpenStream(FileAccess access)
        {
            return new UnmanagedMemoryStream(this, 0, (long)ByteLength, access);
        }
    }
}