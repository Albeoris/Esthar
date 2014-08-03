using System.Runtime.InteropServices;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IdTriangle
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public IdTriangleVertex[] Vertices;
    }
}