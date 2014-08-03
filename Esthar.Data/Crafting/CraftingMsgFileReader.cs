using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class CraftingMsgFileReader : GameFileReader
    {
        public CraftingMsgFileReader(Stream input)
            : base(input)
        {
        }

        public override void Close()
        {
            IOStream.Seek(0, SeekOrigin.Begin);
        }

        public override void Open()
        {
            Close();
        }

        public string ReadString(int offset, int length)
        {
            IOStream.Seek(offset, SeekOrigin.Begin);
            if (length < 0) // Вычисляем длину для последней записи
                length = (int)(IOStream.Length - IOStream.Position);

            byte[] buff = new byte[length - 1]; // Отрезаем {End}
            IOStream.EnsureRead(buff, 0, buff.Length);

            return Options.Encoding.GetString(buff);
        }
    }
}