using System.Runtime.InteropServices;
using System.Windows;

namespace Esthar.Core
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct FontStyleConverter
    {
        [FieldOffset(0)]
        public FontStyle FontStyle;
        [FieldOffset(0)]
        public FontStyleEnum FontStyleEnum;

        public static implicit operator FontStyle(FontStyleConverter converter)
        {
            return converter.FontStyle;
        }

        public static implicit operator FontStyleEnum(FontStyleConverter converter)
        {
            return converter.FontStyleEnum;
        }

        public static implicit operator int(FontStyleConverter converter)
        {
            return (int)converter.FontStyleEnum;
        }

        public static implicit operator FontStyleConverter(FontStyle fontStyle)
        {
            return new FontStyleConverter { FontStyle = fontStyle };
        }

        public static implicit operator FontStyleConverter(FontStyleEnum fontStyleEnum)
        {
            return new FontStyleConverter { FontStyleEnum = fontStyleEnum };
        }

        public static implicit operator FontStyleConverter(int value)
        {
            return new FontStyleConverter { FontStyleEnum = (FontStyleEnum)value };
        }
    }
}