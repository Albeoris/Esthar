using System.Runtime.InteropServices;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct KernellS10Entry
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] Unknown;

        //10 00 00 00 20 10 A3 01 00 00 00 00 00 00 00 00
        //3D 00 00 00 06 00 A3 01 00 FE 01 00 00 00 00 00
        //39 00 00 00 03 00 23 01 00 FE 7E 00 0D 56 00 01
        //11 01 00 00 02 00 23 01 00 FF 20 00 22 00 00 00
        //08 00 00 00 02 00 23 01 00 FE 00 00 00 04 00 00
        //38 00 00 05 02 0E 23 01 00 C8 00 00 00 80 00 00
        //10 01 00 00 0D 04 23 01 00 FF 00 00 00 00 00 00
        //0F 01 00 00 10 10 23 01 00 FF 00 00 00 00 00 00
        //B5 00 00 04 12 01 2B 01 00 00 00 00 00 00 00 00
        //3E 00 00 04 13 00 23 01 00 00 00 00 00 00 01 00
        //B6 00 00 00 11 00 23 01 00 00 00 00 00 00 01 00
        //B7 00 00 00 02 00 23 01 00 FF 00 00 00 00 08 00
    }
}