using System.IO;
using System.IO.MemoryMappedFiles;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class KernellBinFileReader : GameFileReader
    {
        private int _sectionsCount;
        private int[] _sectionsOffsets;
        private int[] _sectionsLengths;

        public KernellBinFileReader(Stream input)
            : base(input)
        {
        }
        
        public override void Close()
        {
            IOStream.Seek(0, SeekOrigin.Begin);

            _sectionsCount = -1;
            _sectionsOffsets = null;
            _sectionsLengths = null;
        }

        public override void Open()
        {
            Close();

            int[] offsets, lengths;
            int count = ReadSectionMetrics(out offsets, out lengths);

            _sectionsCount = count;
            _sectionsOffsets = offsets;
            _sectionsLengths = lengths;
        }

        private int ReadSectionMetrics(out int[] offsets, out int[] lengths)
        {
            BinaryReader br = IOStream.GetBinaryReader();
            int count = br.ReadInt32();

            offsets = new int[count];
            lengths = new int[count];

            for (int i = 0; i < count; i++)
                offsets[i] = br.ReadInt32();

            for (int i = 0; i < count - 1; i++)
                lengths[i] = offsets[i + 1] - offsets[i];

            lengths[count - 1] = (int)IOStream.Length - offsets[count - 1];

            return count;
        }
    }
}