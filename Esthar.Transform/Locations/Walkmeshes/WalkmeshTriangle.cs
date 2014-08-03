using System;
using Esthar.Core;

namespace Esthar.Data.Transform
{
    public struct WalkmeshTriangle
    {
        public Vector3[] Coords;

        public WalkmeshTriangle(Vector3[] coords)
        {
            Coords = Exceptions.CheckArgumentNull(coords, "coords");
            if (Coords.Length != 3)
                throw new ArgumentException("coords");
        }
    }
}