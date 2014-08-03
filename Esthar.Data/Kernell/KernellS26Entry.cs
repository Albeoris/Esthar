using System.Runtime.InteropServices;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct KernellS26Entry
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 60)] public byte[] Unknown;

        //00 00 00 00 00 00 00 00  00 00 01 01 00 00 00 00
        //00 01 01 02 02 02 03 04  09 09 09 0A 0A 0B 0B 0C
        //0C 0D 0D 0E 00 00 01 01  01 02 03 04 05 06 07 08
        //00 01 02 0F 03 04 05 06  07 08 08 0F            
    }
}