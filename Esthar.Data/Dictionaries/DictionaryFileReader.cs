using System.IO;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class DictionaryFileReader : GameFileReader
    {
        public string[] Titles;

        public DictionaryFileReader(Stream input)
            : base(input)
        {
        }

        public override void Close()
        {
            IOStream.Seek(0, SeekOrigin.Begin);

            Titles = null;
        }

        public override void Open()
        {
            Close();

            int[] offsets, lengths;
            int count = ReadMetrics(out offsets, out lengths);

            string[] titles = new string[count];
            for (int i = 0; i < count; i++)
            {
                IOStream.Seek(offsets[i], SeekOrigin.Begin);
                byte[] buff = new byte[lengths[i] - 1]; // Отрезаем {End}
                IOStream.EnsureRead(buff, 0, buff.Length);
                titles[i] = Options.Encoding.GetString(buff);
            }
            Titles = titles;
        }

        private int ReadMetrics(out int[] offsets, out int[] lengths)
        {
            BinaryReader br = IOStream.GetBinaryReader();
            int count = br.ReadUInt16();

            offsets = new int[count];
            lengths = new int[count];

            for (int i = 0; i < count; i++)
                offsets[i] = br.ReadUInt16();

            for (int i = 0; i < count - 1; i++)
                lengths[i] = offsets[i + 1] - offsets[i];

            lengths[count - 1] = (int)IOStream.Length - offsets[count - 1];

            return count;
        }
    }
}