using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esthar.Core;

namespace Esthar.Data.Transform
{
    public sealed class Placeable
    {
        public byte[] Unknown;

        public Placeable(byte[] unknown)
        {
            Unknown = Exceptions.CheckArgumentNull(unknown, "unknown");
            if (Unknown.Length != 8)
                throw new ArgumentException("unknown");
        }
    }
}
