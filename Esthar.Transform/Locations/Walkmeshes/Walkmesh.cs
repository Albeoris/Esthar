using System;
using System.Collections.Generic;
using Esthar.Core;

namespace Esthar.Data.Transform
{
    public sealed class Walkmesh
    {
        public readonly WalkmeshTriangle[] Triangles;
        public readonly WalkmeshPassability[] Passability;

        public Walkmesh(WalkmeshTriangle[] triangles, WalkmeshPassability[] passability)
        {
            Triangles = Exceptions.CheckArgumentNull(triangles, "triangles");
            Passability = Exceptions.CheckArgumentNull(passability, "passability");

            if (Triangles.Length != Passability.Length)
                throw new ArgumentException();
        }
    }
}