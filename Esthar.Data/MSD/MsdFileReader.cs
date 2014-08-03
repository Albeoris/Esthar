using System;
using System.IO;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class MsdFileReader : GameFileReader
    {
        private int[] _offsets;
        private int[] _lengths;

        public int Count;

        public MsdFileReader(Stream input)
            : base(input)
        {
        }

        public override void Close()
        {
            IOStream.Seek(0, SeekOrigin.Begin);
            _offsets = null;
            _lengths = null;
            Count = -1;
        }

        public override void Open()
        {
            Close();

            Count = ReadMetrics(out _offsets, out _lengths);
        }

        public string[] ReadAllMonologues()
        {
            string[] result = new string[Count];

            for (int i = 0; i < Count; i++)
                result[i] = ReadMonologue(i);

            return result;
        }

        public string ReadMonologue(int index)
        {
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException("index");
            
            int offset = _offsets[index];
            int length = _lengths[index];
            if (length < 1)
                return string.Empty;

            byte[] buff = new byte[length];
            IOStream.Seek(offset, SeekOrigin.Begin);
            IOStream.EnsureRead(buff, 0, length);

            return Options.Encoding.GetString(buff);
        }

        private int ReadMetrics(out int[] offsets, out int[] lengths)
        {
            if (IOStream.IsEndOfStream())
            {
                offsets = new int[0];
                lengths = new int[0];
                return 0;
            }

            BinaryReader br = IOStream.GetBinaryReader();

            int currentOffset = br.ReadInt32();
            int count = currentOffset / 4;
            offsets = new int[count];
            lengths = new int[count];

            offsets[0] = currentOffset;
            for (int i = 1; i < count; i++)
            {
                int offset = br.ReadInt32();
                lengths[i - 1] = offset - currentOffset - 1;
                offsets[i] = currentOffset = offset;
            }

            lengths[count - 1] = (int)(br.BaseStream.Length - currentOffset) - 1;

            return count;
        }
    }
}