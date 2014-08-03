using System.Runtime.InteropServices;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct KernellS05LimitRenzokuken
    {
        public const int TextSection = 36;

        public ushort NameOffset;
        public ushort DescriptionOffset;

        public ushort Unknown1;
        public ushort Unknown2;
        public ushort Unknown3;
        public ushort Unknown4;
        public ushort Unknown5;

        public uint UnknownZero1;
        public uint UnknownZero2;
    }
}