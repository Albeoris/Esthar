using System.Runtime.InteropServices;
using Esthar.Core;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MskRect
    {
        public Vector3 Top;
        public Vector3 Bottom;
        public Vector3 Right;
        public Vector3 Left;
    }
}