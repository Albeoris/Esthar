using System.Runtime.InteropServices;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct KernellS12GFCommandAbility
    {
        public const int TextSection = 42;

        public ushort NameOffset;
        public ushort DescriptionOffset;

        public uint Unknown;

        //0000 0600 01020000    0F00 1200 01030000
        //1A00 1F00 01060000    2A00 2F00 01040000
        //3A00 4000 FA000000    4600 4B00 281D0000
        //5500 5A00 3C1E0000    6500 6E00 3C180000
        //7D00 8700 64190000    9500 9C00 64170000
        //A800 B100 641C0000    BE00 C600 C81A0000
        //D400 DB00 50200000    E800 EF00 C81B0000
        //FB00 0301 64210000    1101 1701 64220000
        //2301 2C01 641F0000    3A01 4101 64070000
        //4E01 5601 64260000      
    }
}