using System.Runtime.InteropServices;

namespace Esthar.Core
{
    [UnsafeCastContainsOnlyValueTypes]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Line3
    {
        public Vector3 Begin;
        public Vector3 End;
    }
}