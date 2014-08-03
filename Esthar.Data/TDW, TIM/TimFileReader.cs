using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class TimFileReader : GameFileReader
    {
        public TimHeader Header;
        public TimPalettes Palettes;

        public TimFileReader(Stream input)
            : base(input)
        {
        }

        public override void Close()
        {
            IOStream.Seek(0, SeekOrigin.Begin);
            Header = null;
            Palettes = null;
        }

        public override void Open()
        {
            Close();

            Header = TimHeader.Read(IOStream);
            Palettes = TimPalettes.TryRead(Header, IOStream);
        }

        public TimImage ReadImage()
        {
            return TimImage.Read(Header, IOStream);
        }
    }
}