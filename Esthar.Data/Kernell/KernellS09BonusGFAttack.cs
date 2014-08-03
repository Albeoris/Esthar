using System.Runtime.InteropServices;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct KernellS09BonusGFAttack
    {
        public const int TextSection = 40;

        public ushort NameOffset;
        public ushort DescriptionOffset;

        public byte Unknown01;
        public byte Unknown02;
        public byte Unknown03;
        public byte Unknown04;
        public byte Unknown05;
        public byte Unknown06;
        public byte Unknown07;
        public byte Unknown08;
        public byte Unknown09;
        public byte Unknown10;
        public byte Unknown11;
        public byte Unknown12;
        public byte Unknown13;
        public byte Unknown14;
        public byte Unknown15;

        //0000 BB00 0B 00 FE 49 23 05 01 00 00 00 01 00 00 00 01 01
        //0C00 8C00 0B 1E FF 49 A3 05 01 01 00 00 00 00 00 00 32 FE
        //1A00 6100 0B 28 00 49 23 05 01 01 00 00 00 00 00 00 64 64
        //2400 6200 0B 3C 00 49 23 05 01 00 00 00 00 00 00 00 64 64
        //2F00 6300 0B 50 00 49 23 05 01 00 00 00 00 00 00 00 64 64
        //3B00 6400 0B 64 00 49 2B 05 01 00 00 00 00 00 00 00 64 64
        //4600 6000 18 01 00 09 23 00 01 00 00 00 00 00 00 00 01 01
        //5300 4901 0B 32 00 49 23 04 01 00 00 00 00 00 00 00 64 64
        //5D00 4A01 23 0A 00 49 23 04 01 00 00 00 00 00 00 00 01 01
        //6800 4801 0B 64 FA 49 23 04 01 00 04 00 00 00 00 00 64 64
        //7100 4701 0B 00 FA 49 23 04 01 00 00 00 01 00 00 00 01 01
        //7D00 5B00 01 20 00 59 20 04 01 00 00 00 00 00 00 00 01 FE
        //8500 5D00 20 0A 00 19 23 00 01 00 00 00 00 00 00 00 01 FF
        //9000 5E00 05 00 FF 19 A3 00 01 00 00 00 00 00 01 00 01 01
        //9B00 5C00 17 00 00 19 23 00 01 00 00 00 00 00 00 00 01 01
        //A500 5201 1C 0A 00 59 20 04 01 00 00 00 00 00 00 00 01 FE
    }
}