using System.Runtime.InteropServices;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct KernellS11GFJunctionAbility
    {
        public const int TextSection = 41;

        public ushort NameOffset;
        public ushort DescriptionOffset;

        public ushort Unknown1;
        public ushort Unknown2;

        //FFFF FFFF 0000 0000    0000 0500 3201 0000
        //0F00 1500 3202 0000    2400 2A00 3204 0000
        //3B00 4100 3208 0000    4C00 5200 3210 0000
        //6000 6600 7820 0000    7300 7900 C840 0000
        //8700 8D00 7880 0000    9900 A000 C800 0100
        //AD00 B800 A000 0200    C900 D200 A000 0400
        //E000 EB00 6400 0800    FB00 0401 6400 1000
        //1101 1C01 8200 2000    2E01 3901 B400 4000
        //4B01 5601 8200 8000    6501 7001 B400 0001
        //8101 8B01 9600 0002    9F01 A901 C800 0004
    }
}