using System;
using Esthar.Core;

namespace Esthar.Data.Transform
{
    public struct WalkmeshPassability
    {
        public ushort[] Edges;

        public WalkmeshPassability(ushort[] edges)
        {
            Edges = Exceptions.CheckArgumentNull(edges, "edges");
            if (edges.Length != 3)
                throw new ArgumentException("edges");
        }
    }
}