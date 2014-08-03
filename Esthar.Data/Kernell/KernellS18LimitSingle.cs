using System.Runtime.InteropServices;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct KernellS18LimitSingle
    {
        public const int TextSection = 48;

        public ushort NameOffset;
        public ushort DescriptionOffset;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] Unknown;

        //0000 0900 C8 00 01 50 04 80 40 01 01 00 C8 C8 00 00 C8 00 00 00 00 00
        //1800 2300 E4 00 02 78 05 80 50 01 01 00 C8 C8 00 00 C8 00 00 00 00 00
        //2F00 3900 05 01 01 8C 04 80 40 01 01 00 C8 C8 00 00 C8 00 00 00 00 00
        //4800 5300 06 01 01 19 04 80 50 01 06 00 C8 C8 00 00 C8 00 00 00 00 00
        //5F00 6E00 07 01 01 96 04 80 40 01 01 00 C8 C8 00 00 C8 00 00 00 00 00                        
    }
}