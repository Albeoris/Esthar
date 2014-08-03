using System.Runtime.InteropServices;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct KernellS30Texts
    {
        public const int TextSection = 55;

        public ushort NameOffset;
    }
}