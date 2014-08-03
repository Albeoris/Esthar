using System.Runtime.InteropServices;
using Esthar.Core;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public sealed class CaCamera
    {
        public Vector3 XAxis;
        public Vector3 YAxis;
        public Vector3 ZAxis;
        public short ZAxisZCopy;
        public Vector3Int32 Position;
        public int Blank;
        public short Zoom;
        public short ZoomCopy;
    }
}