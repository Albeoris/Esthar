using System.Runtime.InteropServices;

namespace Esthar.Core
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RectInt16
    {
        public short Top;
        public short Bottom;
        public short Right;
        public short Left;
    }
}