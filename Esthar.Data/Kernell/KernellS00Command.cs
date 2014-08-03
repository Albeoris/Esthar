using System.Runtime.InteropServices;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct KernellS00Command
    {
        public const int TextSection = 31;

        public ushort NameOffset;
        public ushort DescriptionOffset;

        public byte Unknown1;
        public byte Unknown2;
        public byte Unknown3;
        public byte Unknown4;

        //FFFF FFFF FF A0 54 00     0000 0700 FF A0 54 00
        //2300 2900 FF 80 D4 00     3300 3600 FF 81 D4 00
        //4A00 4F00 FF 82 D4 00     5700 6200 FF A0 50 00
        //7800 7D00 FF 83 50 00     8700 8E00 09 A0 50 00
        //9D00 FFFF FF A0 D4 00     A300 A800 FF A0 54 00 
        //B800 BE00 FF A0 D4 00     CE00 D300 FF A0 50 00
        //E600 EA00 FF A0 54 00     0001 FFFF FF A0 54 00 
        //0601 0B01 FF 84 40 00     1B01 2601 FF 86 54 00
        //3601 3B01 FF 85 54 00     5B01 6601 FF 87 54 00
        //7701 7F01 FF 87 50 00     8F01 9701 FF 88 50 00
        //A401 AA01 FF 87 54 00     C101 C701 FF 87 54 00
        //DE01 E401 FF 87 54 00     FB01 0202 0B A0 18 00
        //1D02 2602 03 A0 08 00     4602 5002 02 60 14 00
        //6502 6D02 00 20 14 00     7502 7C02 01 20 15 00
        //8502 8E02 FF A0 54 00     A002 A502 0A A0 50 00
        //B902 BE02 04 A0 54 00     CF02 D802 08 A0 50 00
        //F502 FC02 05 A0 54 00     0503 0D03 06 A0 50 00
        //2003 2603 07 A0 50 00     3803 3F03 FF 80 54 00
        //4903 5003 FF 80 54 00     5B03 6203 FF 80 54 00
        //7003 7803 FF A0 08 00
    }
}