using System.Runtime.InteropServices;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct KernellS01Magic
    {
        public const int TextSection = 32;

        public ushort NameOffset;
        public ushort DescriptionOffset;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 56)]
        public byte[] Unknown3;
    }
}