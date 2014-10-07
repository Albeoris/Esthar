using System;
using System.Collections.Generic;
using System.IO;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class JsmFileWriter : IDisposable
    {
        private readonly Stream _output;

        public JsmFileWriter(Stream output)
        {
            _output = Exceptions.CheckWritableStream(output, "output");
        }

        public void Dispose()
        {
            _output.SafeDispose();
        }

        public void WriteScripts(JsmHeader header, IEnumerable<JsmGroup> groups, IEnumerable<JsmScript> scripts, IEnumerable<JsmOperation> opertations)
        {
            BinaryWriter bw = new BinaryWriter(_output);

            _output.WriteStruct(header);

            foreach (JsmGroup group in groups)
                bw.Write((ushort)((group.Label << 7) | (group.ScriptsCount - 1)));

            foreach (JsmScript script in scripts)
                bw.Write((ushort)(script.Position | ((script.Flag ? 1 : 0) << 15)));

            foreach (JsmOperation operation in opertations)
                bw.Write(operation.Operation);
        }
    }
}