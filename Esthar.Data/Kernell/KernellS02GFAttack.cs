using System.Runtime.InteropServices;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct KernellS02GFAttack
    {
        public const int TextSection = 33;

        public ushort NameOffset;
        public ushort DescriptionOffset;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] Unknown4;
    }
}