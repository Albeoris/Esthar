using System.Runtime.InteropServices;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct KernellS24LimitSorcerer
    {
        public const int TextSection = 52;

        public ushort NameOffset;
        public ushort DescriptionOffset;

        public uint Unknown;

        //0000 0300 A0D4FF00
        //1600 2100 A098FF00 
    }
}