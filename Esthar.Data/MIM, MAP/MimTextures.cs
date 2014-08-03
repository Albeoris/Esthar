using System.IO;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class MimTextures
    {
        public const int AtlasWidth = 1664;

        private readonly Stream _input;

        public MimTextures(Stream input)
        {
            Exceptions.CheckArgumentNull(input, "input");

            _input = input;
        }

        public byte[] ReadIndices(MimTile tile)
        {
            Exceptions.CheckArgumentNull(tile, "tile");

            byte[] result = new byte[16 * 16];

            _input.Seek(tile.Layered.GetPositionInTexture(), SeekOrigin.Begin);
            Stream input = tile.Layered.IsFullByteIndices ? _input : new HalfByteStream(_input);
            for (int i = 0; i < 16; i++)
            {
                input.Read(result, i * 16, 16);
                _input.Seek(AtlasWidth - 16, SeekOrigin.Current);
            }

            return result;
        }
    }
}