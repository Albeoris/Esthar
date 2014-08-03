using System.Runtime.InteropServices;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct KernellS29Entry
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 60)] public byte[] Unknown;

        //32 78 78 32 50 78 78 78 19 50 3C 1E 2C 2C 0A C8
        //00 1E 00 1E 1E 00 00 00 00 00 0F 00 00 00 00 00
        //C8 00 2D 00 1E 00 1E 00 00 00 00 00 00 00 00 00
        //46 00 64 00 8C 01 B4 01 3C 5A 96 C8                                    
    }
}