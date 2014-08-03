using System.Runtime.InteropServices;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct KernellS03EnemyCommand
    {
        public const int TextSection = 34;

        public ushort NameOffset;
        public ushort DescriptionOffset;

        public ushort Unknown3;
        public ushort Unknown4;
        public ushort Unknown5;
        public ushort Unknown6;
        public ushort Unknown7;
        public ushort Unknown8;
        public ushort Unknown9;
        public ushort Unknown10;
    }
}