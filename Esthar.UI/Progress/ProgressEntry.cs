using System;
using System.Threading;

namespace Esthar.UI
{
    public sealed class ProgressEntry
    {
        public readonly object Key;
        private long _processedSize, _totalSize;

        public ProgressEntry(object key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            Key = key;
        }

        public long ProcessedSize
        {
            get { return _processedSize; }
        }

        public long TotalSize
        {
            get { return _totalSize; }
        }

        public long UpdateProcessedSize(long value)
        {
            return value - Interlocked.Exchange(ref _processedSize, value);
        }

        public long UpdateTotalSize(long value)
        {
            return value - Interlocked.Exchange(ref _totalSize, value);
        }
    }
}