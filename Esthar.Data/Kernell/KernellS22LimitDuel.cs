using System.Runtime.InteropServices;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct KernellS22LimitDuel
    {
        public const int TextSection = 51;

        public ushort NameOffset;
        public ushort DescriptionOffset;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 28)] public byte[] Unknown;

        //0000 0B00 12 01 01 10 04 80 58 20 01 00 00 00 20 00 40 00 FF FF FF FF FF FF 00 00 00 00 00 00
        //1700 1D00 12 01 01 12 04 80 50 20 01 00 00 00 00 20 00 80 FF FF FF FF FF FF 00 00 00 00 00 00
        //2900 3300 12 01 01 14 04 80 50 20 01 00 00 00 00 10 00 40 FF FF FF FF FF FF 00 00 00 00 00 00
        //3F00 4900 12 01 01 18 04 80 50 20 01 00 00 00 00 80 00 80 20 00 FF FF FF FF 00 00 00 00 00 00
        //5500 6200 12 01 01 1C 04 80 50 20 01 00 00 00 04 00 08 00 04 00 08 00 FF FF 00 00 00 00 00 00
        //6E00 7C00 12 01 07 04 00 80 50 20 01 00 00 00 00 40 20 00 00 10 20 00 FF FF 00 00 00 00 00 00
        //8800 9500 D4 00 01 30 04 80 40 20 01 00 00 00 00 41 00 40 00 40 00 40 20 00 00 00 00 00 00 00
        //A400 B200 70 00 01 34 04 80 50 20 01 00 00 00 00 11 40 00 00 40 10 00 20 00 00 00 00 00 00 00
        //BE00 CD00 D8 00 01 48 04 80 50 20 01 00 00 00 10 01 80 00 40 00 20 00 00 10 00 00 00 00 00 00
        //D900 E900 D7 00 01 32 04 80 40 20 01 00 00 00 00 11 00 20 00 40 00 80 10 00 00 00 00 00 00 00
    }
}