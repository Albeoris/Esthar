using System.Runtime.InteropServices;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct KernellS08Item2
    {
        public const int TextSection = 39;

        public ushort NameOffset;
        public ushort DescriptionOffset;
    }
}