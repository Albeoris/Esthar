using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using Esthar.Core;

namespace Esthar.Data
{
    public abstract class GameFileReader : IDisposable
    {
        protected Stream IOStream { get; private set; }

        protected GameFileReader(Stream stream)
        {
            IOStream = Exceptions.CheckArgumentNull(stream, "stream");
            try
            {
                Open();
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        public void Dispose()
        {
            Close();
            IOStream.NullSafeDispose();
        }

        public abstract void Open();
        public abstract void Close();
    }
}