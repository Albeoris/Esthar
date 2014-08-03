using System;
using Esthar.Core;

namespace Esthar.Data.Transform
{
    public sealed class LocationReadersState : IDisposable
    {
        private readonly LocationReaders _readers;

        public LocationReadersState(LocationReaders readers)
        {
            _readers = Exceptions.CheckArgumentNull(readers, "readers");
        }

        public void Dispose()
        {
            if (!_readers.EndRead())
                throw new Exception("Failed to end read.");
        }
    }
}