using System.Runtime.InteropServices;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct KernellS07Item1
    {
        public const int TextSection = 38;

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
        public byte Unknown11;
        public byte Unknown12;
        public byte Unknown13;
        public byte Unknown14;
        public byte Unknown15;
        public byte Unknown16;
        public byte Unknown17;
        public byte Unknown18;
        public byte Unknown19;
        public byte Unknown20;

        //FFFF FFFF 00 00 00 00 80 00 02 00 00 00 00 00 00 00 00 00 FF 01 01 00
        //0000 0700 32 00 04 04 00 14 E2 00 00 00 00 00 00 00 00 00 FF 01 01 00
        //1300 1B00 32 00 04 08 00 14 E2 00 00 00 00 00 00 00 00 00 FF 01 01 00
        //2700 3100 33 00 04 14 00 14 E2 00 00 00 00 00 00 00 00 00 FF 01 01 00
        //3E00 4900 33 00 04 28 00 14 E2 00 00 00 00 00 00 00 00 00 FF 01 01 00
        //5600 5F00 34 00 20 10 00 14 E2 00 00 00 00 00 00 00 00 00 FF 01 01 00
        //6900 7500 35 00 04 14 00 04 E2 00 00 00 00 00 00 00 00 00 FF 01 01 00
        //8F00 9C00 05 00 05 00 00 15 E2 00 00 C8 01 00 00 00 00 00 FF 01 01 00
        //A900 B600 07 00 05 00 00 05 E2 00 00 C8 01 00 00 00 00 00 FF 01 01 00
        //D300 DA00 3A 00 20 10 00 14 E2 00 00 C8 7E 00 0D 56 00 01 FF 01 01 00
        //F800 0201 3B 00 20 10 00 04 E2 00 00 C8 7E 00 0D 56 00 01 FF 01 01 00
        //2501 2E01 26 01 04 00 40 14 E2 00 00 C8 02 00 00 00 00 00 FF 01 01 00
        //3801 3D01 24 01 04 00 40 14 E2 00 00 C8 04 00 00 10 00 00 FF 01 01 00
        //4901 5301 25 01 04 00 40 14 E2 00 00 C8 08 00 00 00 00 00 FF 01 01 00
        //5E01 6A01 27 01 04 00 40 14 E2 00 00 C8 10 00 00 00 00 00 FF 01 01 00
        //7501 8001 28 01 04 00 40 14 E2 00 00 C8 40 00 00 02 00 00 FF 01 01 00
        //9201 9901 43 00 04 00 40 14 E2 00 00 C8 7E 00 01 52 00 00 FF 01 01 00
        //A801 B001 43 00 04 00 40 14 E2 00 00 C8 7E 00 0D 56 00 01 FF 01 01 00
        //CD01 D801 35 01 02 00 40 10 A2 00 00 FF 00 00 00 08 00 00 80 01 01 00
        //F801 FD01 35 01 02 00 40 10 A2 00 00 FF 00 00 00 08 00 00 FF 01 01 00
        //1402 2302 36 01 02 00 40 00 A2 00 00 FF 00 00 00 08 00 00 80 01 01 00
        //4902 5202 36 01 02 00 40 00 A2 00 00 FF 00 00 00 08 00 00 FF 01 01 00
        //6F02 7B02 21 00 02 00 40 14 A1 00 00 FE 00 00 40 00 00 00 FF 01 01 00
        //8C02 9A02 20 00 02 00 40 14 A1 00 00 FE 00 00 20 00 00 00 FF 01 01 00
        //AE02 B902 6F 00 02 00 40 14 A1 00 00 FE 00 00 00 01 00 00 FF 01 01 00
        //CA02 D602 0E 00 02 00 80 54 A1 05 00 C8 01 00 00 00 00 00 FF 01 01 00
        //E802 F302 AF 00 02 30 80 54 A1 05 00 00 00 00 00 00 00 00 FF 01 01 80
        //0403 1003 7C 00 02 30 80 54 A1 05 00 00 00 00 00 00 00 00 FF 01 01 00
        //2103 2E03 DF 00 02 14 80 46 A1 05 00 00 00 00 00 00 00 00 FF 01 0A 00
        //4103 4E03 95 00 02 3C 80 44 A1 05 00 00 00 00 00 00 00 00 FF 01 01 00
        //6103 6F03 02 00 0E 00 80 48 A2 00 00 00 00 00 00 00 00 00 FF 00 01 00
        //8103 9003 01 00 0E 00 80 48 A2 00 00 FF 00 00 00 00 00 00 FF 00 01 00
        //A603 B103 0F 00 0E 00 80 50 A2 00 00 FF 00 00 00 00 00 00 FF 00 01 00
    }
}