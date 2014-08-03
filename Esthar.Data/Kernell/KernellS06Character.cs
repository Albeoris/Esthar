using System.Runtime.InteropServices;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct KernellS06Character
    {
        public const int TextSection = 37;

        //FFFF FA00 0516 6400 2CFF B300 1E05 0226 1A05 0529 1C04 0627 1805 0529 0005 141E 0007 0F0E
        //0000 FA00 0B00 6400 2AFF D200 1D05 0727 1705 042A 1802 0328 1406 002E 0005 1414 0008 0E10
        //0500 FA00 0E00 6400 29FF AC00 1C16 052E 1606 052B 1B04 0526 1604 0126 0004 1314 0008 0D18
        public ushort NameOffset;

        public ushort Unknown02;
        public ushort Unknown03;
        public ushort Unknown04;
        public ushort Unknown05;
        public ushort Unknown06;
        public ushort Unknown07;
        public ushort Unknown08;
        public ushort Unknown09;
        public ushort Unknown11;
        public ushort Unknown12;
        public ushort Unknown13;
        public ushort Unknown14;
        public ushort Unknown15;
        public ushort Unknown16;
        public ushort Unknown17;
        public ushort Unknown18;
    }
}