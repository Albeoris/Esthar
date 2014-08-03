using System.Runtime.InteropServices;
using Esthar.Core;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct InfGateway
    {
        public Line3 SourceLine;
        public Vector3 TargetPoint;
        public ushort TargetFieldID;
        public int Unknown1;
        public int Unknown2;
        public int Unknown3;
    }
}