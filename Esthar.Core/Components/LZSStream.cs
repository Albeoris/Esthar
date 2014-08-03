using System;
using System.IO;

namespace Esthar.Core
{
    public sealed class LZSStream
    {
        private readonly Stream _input;
        private readonly Stream _output;
        private readonly CircularBuffer<byte> _circularBuffer;

        public event EventHandler<int> ReverseProgress;

        public LZSStream(Stream input, Stream output)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            if (output == null)
                throw new ArgumentNullException("output");

            if (!input.CanRead)
                throw new ArgumentException("Входной поток не поддерживает чтения.", "input");
            if (!output.CanWrite)
                throw new ArgumentException("Выходной поток не поддерживает записи.", "input");

            _input = input;
            _output = output;
            _circularBuffer = new CircularBuffer<byte>(4096);
        }

        public void Decompress(int unpackedLength)
        {
            byte bits = 0, bitsCount = 0;

            while (unpackedLength != 0)
            {
                int b = _input.ReadByte();
                if (b == -1)
                    throw new Exception("Непредвиденный конец входного потока.");

                if (_input.Position % 256 == 0)
                    ReverseProgress.NullSafeInvoke(this, unpackedLength);

                byte current = (byte)b;

                if (bitsCount == 0)
                {
                    bits = current;
                    bitsCount = 8;
                    continue;
                }

                if ((bits & 1) != 0)
                {
                    _output.WriteByte(current);
                    _circularBuffer.Write(current);
                    unpackedLength--;
                }
                else
                {
                    short offset = current;

                    b = _input.ReadByte();
                    if (b == -1)
                        throw new Exception("Непредвиденный конец входного потока.");

                    current = (byte)b;

                    offset += (short)((current & 0xF0) << 4);
                    short length = (short)((current & 0xF) + 3);

                    for (int i = offset + 18; --length >= 0; i++)
                    {
                        i &= 0xFFF;
                        current = _circularBuffer.GetByOffset(i);
                        _output.WriteByte(current);
                        _circularBuffer.Write(current);
                        unpackedLength--;
                    }
                }

                bits >>= 1;
                bitsCount--;
            }

            ReverseProgress.NullSafeInvoke(this, unpackedLength);
        }
    }
}