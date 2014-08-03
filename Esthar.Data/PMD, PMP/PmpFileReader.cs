using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class PmpFileReader : GameFileReader
    {
        public PmpHeader Header;

        public PmpFileReader(Stream input)
            : base(input)
        {
        }

        public override void Close()
        {
            IOStream.Seek(0, SeekOrigin.Begin);
            Header = null;
        }

        public override void Open()
        {
            Close();

            if (IOStream.Length == 4 || IOStream.Length == 0)
                return;

            Header = PmpHeader.Read(IOStream);
        }

        public BitmapSource ReadImage(int paletteId)
        {
            using (SafeHGlobalHandle buff = IOStream.ReadBuff((int)(IOStream.Length - IOStream.Position)))
                return BitmapSource.Create(256, buff.Length / 128, 96, 96, PixelFormats.Indexed4, Header.Palettes[paletteId], buff.DangerousGetHandle(), buff.Length, 128);
        }
    }
}