using System.Collections.Generic;
using System.Linq;

namespace Esthar.Data.Transform
{
    public sealed class AsmSegments : List<AsmSegment>
    {
        public AsmSegments()
        {
        }

        public AsmSegments(int capacity)
            : base(capacity)
        {
        }

        public AsmSegments(IEnumerable<AsmSegment> operations)
            : base(operations)
        {
        }

        public AsmSegment GetSegmentByOffset(int offset)
        {
            return this.FirstOrDefault(s => s.Offset <= offset && s.Offset + s.Length > offset);
        }
    }
}