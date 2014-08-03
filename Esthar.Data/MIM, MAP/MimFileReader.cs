using System.IO;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class MimFileReader : GameFileReader
    {
        private readonly DisposableStack _disposables = new DisposableStack();

        public MimPalettes Palettes { get; private set; }
        public MimTextures Textures { get; private set; }

        public MimFileReader(Stream input)
            : base(input)
        {
        }

        public override void Close()
        {
            IOStream.Seek(0, SeekOrigin.Begin);
            Palettes = null;
            Textures = null;
            _disposables.Dispose();
        }

        public override void Open()
        {
            Close();

            using (Stream palettesStream = IOStream.GetStreamSegment(4096, MimPalettes.Size))
                Palettes = MimPalettes.Read(palettesStream);

            Stream textureStream = _disposables.Add(IOStream.GetStreamSegment(12288));
            Textures = new MimTextures(textureStream);
        }
    }
}