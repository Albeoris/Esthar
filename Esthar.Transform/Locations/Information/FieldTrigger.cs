using System.Runtime.InteropServices;
using Esthar.Core;

namespace Esthar.Data.Transform
{
    public struct FieldTrigger
    {
        public byte DoorID;
        public Line3 Boundary;

        public FieldTrigger(byte doorId, Line3 boundary)
        {
            DoorID = doorId;
            Boundary = boundary;
        }
    }
}