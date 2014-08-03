using System.Runtime.InteropServices;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct KernellS21LimitShoot
    {
        public const int TextSection = 50;

        public ushort NameOffset;
        public ushort DescriptionOffset;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)] public byte[] Unknown;

        //0000 0C00 BC 00 01 11 04 80 50 21 01 00 00 00 00 00 65 19 00 00 00 00
        //1800 2500 C0 00 01 0E 04 80 48 21 01 00 00 00 00 00 66 19 00 00 00 00
        //3400 3E00 C1 00 01 0E 04 80 50 21 01 00 00 C8 1A 00 67 19 05 00 00 00
        //5A00 6500 C2 00 01 28 04 80 48 21 01 01 64 00 00 00 68 19 00 00 00 00
        //7800 8600 C3 00 01 3C 04 80 50 21 01 00 00 00 00 00 69 19 00 00 00 00
        //9200 9D00 C4 00 01 07 04 80 50 21 01 00 00 00 00 00 6A 19 00 00 00 00
        //A900 B400 C5 00 24 50 04 80 50 21 01 00 00 00 00 00 6B 19 00 00 00 00
        //D100 DC00 C6 00 01 78 04 80 58 21 01 00 00 00 00 00 6C 19 00 00 00 00                        
    }
}