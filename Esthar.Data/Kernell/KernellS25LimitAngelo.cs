using System.Runtime.InteropServices;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct KernellS25LimitAngelo
    {
        public const int TextSection = 53;

        public ushort NameOffset;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 18)] public byte[] Unknown;

        //0000 56 00 01 48 04 80 48 23 01 00 C8 C8 00 00 00 00 00 00
        //0A00 57 00 01 78 04 80 50 23 01 00 C8 C8 00 00 00 00 00 00
        //1400 58 00 02 00 00 80 08 23 01 00 C8 FE 00 00 00 08 00 00
        //2400 59 00 02 82 05 80 40 23 08 00 C8 C8 00 00 00 00 00 00
        //3100 54 00 02 00 00 80 18 23 01 00 C8 FF 00 00 00 00 00 02
    }
}