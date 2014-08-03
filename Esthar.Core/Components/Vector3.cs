using System;
using System.Runtime.InteropServices;

namespace Esthar.Core
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vector3
    {
        public Int16 X;
        public Int16 Y;
        public Int16 Z;
    }
}