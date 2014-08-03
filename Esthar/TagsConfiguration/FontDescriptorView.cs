using System.Windows;
using System.Windows.Media;
using Esthar.Core;

namespace Esthar
{
    internal sealed class FontDescriptorView : DependencyObject
    {
        public FontDescriptorView(FontDescriptor font)
        {
            IsDefault = font.IsDefault;
            FontFamily = font.FontFamily;
            FontWeight = font.FontWeight;
            FontStyle = font.FontStyle;
            FontStretch = font.FontStretch;
            TextDecorations = font.TextDecorations;
            FontSize = font.FontSize;
        }

        public static readonly DependencyProperty IsDefaultProperty = DependencyProperty.Register("IsDefault", typeof(bool), typeof(FontDescriptorView), new PropertyMetadata(default(bool)));
        public static readonly DependencyProperty FontFamilyProperty = DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(FontDescriptorView), new PropertyMetadata(default(FontFamily)));
        public static readonly DependencyProperty FontWeightProperty = DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(FontDescriptorView), new PropertyMetadata(default(FontWeight)));
        public static readonly DependencyProperty FontStyleProperty = DependencyProperty.Register("FontStyle", typeof(FontStyle), typeof(FontDescriptorView), new PropertyMetadata(default(FontStyle)));
        public static readonly DependencyProperty FontStretchProperty = DependencyProperty.Register("FontStretch", typeof(FontStretch), typeof(FontDescriptorView), new PropertyMetadata(default(FontStretch)));
        public static readonly DependencyProperty TextDecorationsProperty = DependencyProperty.Register("TextDecorations", typeof(TextDecorationCollection), typeof(FontDescriptorView), new PropertyMetadata(default(TextDecorationCollection)));
        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register("FontSize", typeof(double), typeof(FontDescriptorView), new PropertyMetadata(default(double)));

        public bool IsDefault
        {
            get { return (bool)GetValue(IsDefaultProperty); }
            set { SetValue(IsDefaultProperty, value); }
        }

        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        public FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }

        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        public FontStretch FontStretch
        {
            get { return (FontStretch)GetValue(FontStretchProperty); }
            set { SetValue(FontStretchProperty, value); }
        }

        public TextDecorationCollection TextDecorations
        {
            get { return (TextDecorationCollection)GetValue(TextDecorationsProperty); }
            set { SetValue(TextDecorationsProperty, value); }
        }

        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public static explicit operator FontDescriptor(FontDescriptorView self)
        {
            return new FontDescriptor
            {
                FontFamily = self.FontFamily,
                FontWeight = self.FontWeight,
                FontStyle = self.FontStyle,
                FontStretch = self.FontStretch,
                TextDecorations = self.TextDecorations,
                FontSize = self.FontSize
            };
        }

        public static explicit operator FontDescriptorView(FontDescriptor self)
        {
            return new FontDescriptorView(self);
        }
    }
}