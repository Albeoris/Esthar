using System.Runtime.InteropServices;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct KernellS17GFActiveAbility
    {
        public const int TextSection = 47;

        public ushort NameOffset;
        public ushort DescriptionOffset;

        public byte Unknown1;
        public byte Unknown2;
        public byte Unknown3;
        public byte Unknown4;

        //0000 0700 96 FF 00 00    1F00 2900 96 FF 00 00 
        //3B00 4400 96 FF 00 00    5900 6300 C8 80 00 00 
        //7C00 8600 96 81 00 00    A400 AD00 1E 00 00 06 
        //CD00 D600 1E 00 07 0D    F200 FB00 1E 00 0E 17 
        //1201 1B01 1E 00 18 2C    3C01 4801 1E 00 2D 3A 
        //6501 6F01 1E 00 3B 4B    8501 9101 1E 00 4C 5F 
        //AB01 B901 1E 00 60 65    D601 E301 1E 01 00 08 
        //0302 0D02 1E 01 09 14    3002 3802 1E 01 15 24 
        //5202 5A02 1E 01 6F 8E    6F02 7D02 C8 01 25 38 
        //9E02 AD02 1E 01 39 44    D302 E002 1E 01 45 6E
        //0A03 1503 3C 02 00 03    3203 3E03 3C 02 04 09
        //5C03 6603 78 03 00 0B    7703 8003 50 04 00 6D
    }
}