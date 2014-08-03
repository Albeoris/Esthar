using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class MsdFileWriter : IDisposable
    {
        private readonly Stream _output;

        public MsdFileWriter(Stream output)
        {
            _output = Exceptions.CheckWritableStream(output, "output");
        }

        public void Dispose()
        {
            _output.SafeDispose();
        }

        public void WriteAllMonologues(IEnumerable<string> strings)
        {
            List<byte[]> encoded = EncodeStrings(strings);
            WriteMetrics(encoded);
            WriteMonologues(encoded);
        }

        private List<byte[]> EncodeStrings(IEnumerable<string> strings)
        {
            return strings.Select(str => Options.Encoding.GetBytes(str)).ToList();
        }

        private void WriteMetrics(List<byte[]> encoded)
        {
            BinaryWriter bw = _output.GetBinaryWriter();

            int offset = encoded.Count * 4;
            foreach (byte[] data in encoded)
            {
                bw.Write(offset);
                offset += data.Length + 1;
            }
        }

        private void WriteMonologues(List<byte[]> encoded)
        {
            foreach (byte[] data in encoded)
            {
                _output.Write(data, 0, data.Length);
                _output.WriteByte(0); // Terminator
            }
        }
    }
}