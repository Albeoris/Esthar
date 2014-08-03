using System.Windows.Media;

namespace Esthar.Core
{
    public static class ColorExm
    {
        public static string ToHexString(this Color color)
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", color.A, color.R, color.G, color.B);
        }
    }
}