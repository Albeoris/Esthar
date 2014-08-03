using System.Collections.Generic;

namespace Esthar.Data.Transform
{
    public sealed class Placeables : List<Placeable>
    {
        public Placeables()
        {
        }

        public Placeables(int capacity)
            : base(capacity)
        {
        }

        public Placeables(IEnumerable<Placeable> collection)
            : base(collection)
        {
        }
    }
}