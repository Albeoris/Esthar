using System.Runtime.InteropServices;
using Esthar.Core;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IdTriangleVertex
    {
        public Vector3 Point;
        public short Padding;
    }
}