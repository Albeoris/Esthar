using System;
using System.Runtime.InteropServices;

namespace Esthar.Core
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vector3Int32
    {
        public Int32 X;
        public Int32 Y;
        public Int32 Z;
    }
}