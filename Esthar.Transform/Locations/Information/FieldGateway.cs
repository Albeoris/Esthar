using System.Runtime.InteropServices;
using Esthar.Core;

namespace Esthar.Data.Transform
{
    public struct FieldGateway
    {
        public readonly Line3 Boundary;
        public readonly ushort DestinationId;
        public readonly Vector3 DestinationPoint; 
        public readonly int? Unknown1;
        public readonly int? Unknown2;
        public readonly int Unknown3;

        public FieldGateway(Line3 boundary, ushort destinationId, Vector3 destinationPoint, int? unknown1, int? unknown2, int unknown3)
        {
            Boundary = boundary;
            DestinationId = destinationId;
            DestinationPoint = destinationPoint;
            Unknown1 = unknown1;
            Unknown2 = unknown2;
            Unknown3 = unknown3;
        }
    }
}