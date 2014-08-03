using System.IO;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class TdwFileReader : GameFileReader
    {
        public byte[] Table;
        public TimFileReader TimReader;

        public TdwFileReader(Stream input)
            : base(input)
        {
        }

        public override void Close()
        {
            IOStream.Seek(0, SeekOrigin.Begin);
            TimReader.NullSafeDispose();
            TimReader = null;
            Table = null;
        }

        public override void Open()
        {
            Close();

            BinaryReader br = IOStream.GetBinaryReader();

            int tableOffset = br.ReadInt32();
            int dataOffset = br.ReadInt32();
            int tableSize = dataOffset - tableOffset;

            byte[] table = null;
            if (tableSize != 0)
            {
                using (StreamSegment tableSegment = IOStream.GetStreamSegment(tableOffset, tableSize))
                using (HalfByteStream tableStream = new HalfByteStream(tableSegment))
                {
                    table = new byte[tableSize * 2];
                    tableStream.EnsureRead(table, 0, table.Length);
                }
            }

            Table = table;
            TimReader = new TimFileReader(IOStream.GetStreamSegment(IOStream.Position));
        }
    }
}