using System.Runtime.InteropServices;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct KernellS15GFPartyAbility
    {
        public const int TextSection = 45;

        public ushort NameOffset;
        public ushort DescriptionOffset;

        public byte Unknown1;
        public byte Unknown2;
        public byte Unknown3;
        public byte Unknown4;

        //0000 0400 C8 01 00 00    1700 2000 C8 02 00 00 
        //3B00 4300 C8 04 00 00    5E00 6C00 FA 08 00 00 
        //8E00 9400 64 10 00 00    BB00 C600 A0 00 00 01 
        //E700 F200 C8 00 00 02    0001 0901 64 80 00 00 
        //2001 2A01 64 00 01 00    4101 4B01 64 00 02 00 
        //6301 6D01 64 00 04 00    8401 8E01 64 00 08 00 
        //A501 B201 FA 00 40 00    D801 E301 FA 00 20 00 
        //0602 1302 FA 00 10 00    3802 4302 FA 00 80 00 
        //6602 7202 96 00 00 04    9602 A102 FA 20 00 00 
        //BA02 C502 FA 40 00 00    DE02 E502 01 00 00 08 
    }
}