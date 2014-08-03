using System;
using System.IO;
using Esthar.Core;

namespace Esthar.Data
{
    //TODO: Write image
    public sealed class TdwFileWriter : IDisposable
    {
        private readonly Stream _output;

        public TdwFileWriter(Stream output)
        {
            _output = output;
        }

        public void Dispose()
        {
            _output.SafeDispose();
        }

        public void WriteFontCharactersWidths(byte[] charactersWidths)
        {
            BinaryWriter bw = _output.GetBinaryWriter();

            int tableSize = charactersWidths.Length / 2;

            const int tableOffset = 8;
            int dataOffset = tableOffset + tableSize;

            bw.Write(tableOffset);
            bw.Write(dataOffset);

            if (tableSize != 0)
            {
                using (HalfByteStream tableStream = new HalfByteStream(_output))
                    tableStream.Write(charactersWidths, 0, charactersWidths.Length);
            }
        }
    }
}