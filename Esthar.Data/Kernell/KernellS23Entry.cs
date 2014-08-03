using System.Runtime.InteropServices;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct KernellS23Entry
    {
        public byte Unknown1;
        public byte Unknown2;
        public byte Unknown3;
        public byte Unknown4;

        //                         00 01 03 11 01 00 02 06
        //02 01 07 13 03 14 12 01  03 00 11 0D 04 00 01 04
        //05 09 02 05 03 0F 0A 06  00 04 05 0B 01 08 05 0B
        //02 09 0C 06 08 FF FF FF  04 09 06 0B 07 FF FF FF
        //02 00 04 0D 00 02 13 10  09 FF FF FF 04 0E 03 0D
        //04 01 02 06 05 16 12 10  00 15 11 17 02 03 12 18
        //01 15 12 18 05 15 03 18  06 FF FF FF            
    }
}