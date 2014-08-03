using System.Collections.Generic;

namespace Esthar.Data.Transform
{
    public sealed class FieldCameras : List<FieldCamera>
    {
        public FieldCameras()
        {
        }

        public FieldCameras(int capacity)
            : base(capacity)
        {
        }

        public FieldCameras(IEnumerable<FieldCamera> collection)
            : base(collection)
        {
        }
    }
}