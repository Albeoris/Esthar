namespace Esthar.Core
{
    public sealed class FF8TextDecoder
    {
        private readonly FF8TextEncodingCodepage _codepage;

        public FF8TextDecoder(FF8TextEncodingCodepage codepage)
        {
            _codepage = Exceptions.CheckArgumentNull(codepage, "codepage");
        }

        public int GetMaxCharCount(int byteCount)
        {
            return byteCount * FF8TextTag.MaxTagLength;
        }

        public int GetCharCount(byte[] bytes, int index, int count)
        {
            int result = 0;

            char[] buff = new char[FF8TextTag.MaxTagLength];
            while (count > 0)
            {
                FF8TextTag tag = FF8TextTag.TryRead(bytes, ref index, ref count);
                if (tag != null)
                {
                    int offset = 0;
                    result += tag.Write(buff, ref offset);
                }
                else
                {
                    count--;
                    result++;
                    index++;
                }
            }

            return result;
        }

        public int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            int result = 0;

            while (byteCount > 0)
            {
                FF8TextTag tag = FF8TextTag.TryRead(bytes, ref byteIndex, ref byteCount);
                if (tag != null)
                {
                    result += tag.Write(chars, ref charIndex);
                }
                else
                {
                    chars[charIndex++] = _codepage[bytes[byteIndex++]];
                    byteCount--;
                    result++;
                }
            }

            return result;
        }
    }
}