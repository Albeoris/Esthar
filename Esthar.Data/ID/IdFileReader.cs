using System.IO;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class IdFileReader : GameFileReader
    {
        public IdTriangle[] Triangles;
        public IdAccess[] Accesses;

        public IdFileReader(Stream input)
            : base(input)
        {
        }

        public override void Close()
        {
            IOStream.Seek(0, SeekOrigin.Begin);
            Triangles = null;
            Accesses = null;
        }

        public override void Open()
        {
            Close();
            
            int count = IOStream.ReadStruct<int>();

            Triangles = IOStream.ReadStructs<IdTriangle>(count);
            Accesses = IOStream.ReadStructs<IdAccess>(count);
        }
    }
}