using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Esthar.Core;
using Esthar.Data.Transform;
using Esthar.UI;

namespace Esthar
{
    public sealed class LocationView : DependencyObject, IUserTagsHandler
    {
        public LocationView(Location location)
        {
            Location = location;
            UiService.TagsChanged += OnGlobalTagsChanged;
        }

        private void OnGlobalTagsChanged()
        {
            Tags.CollectionChanged -= OnTagsCollectionChanged; 
            Tags.TryRemove(Tags.Except(Options.UserTags));
            Tags.CollectionChanged += OnTagsCollectionChanged;
            ConstructFontDescriptor();
        }

        private void OnTagsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ConstructFontDescriptor();
        }

        public static readonly DependencyProperty LocationProperty = DependencyProperty.Register("Location", typeof(Location), typeof(LocationView), new PropertyMetadata(null, OnPropertyChanged));
        public static readonly DependencyProperty TagsProperty = DependencyProperty.Register("Tags", typeof(UserTagCollection), typeof(LocationView), new PropertyMetadata(null, OnPropertyChanged));

        public static readonly DependencyProperty BackgroundBrushProperty = DependencyProperty.Register("BackgroundBrush", typeof(Brush), typeof(LocationView), new PropertyMetadata(default(Brush)));
        public static readonly DependencyProperty ForegroundBrushProperty = DependencyProperty.Register("ForegroundBrush", typeof(Brush), typeof(LocationView), new PropertyMetadata(default(Brush)));
        public static readonly DependencyProperty FontFamilyProperty = DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(LocationView), new PropertyMetadata(default(FontFamily)));
        public static readonly DependencyProperty FontWeightProperty = DependencyProperty.Register("FontWeight", typeof(FontWeight?), typeof(LocationView), new PropertyMetadata(default(FontWeight?)));
        public static readonly DependencyProperty FontStyleProperty = DependencyProperty.Register("FontStyle", typeof(FontStyle?), typeof(LocationView), new PropertyMetadata(default(FontStyle?)));
        public static readonly DependencyProperty FontStretchProperty = DependencyProperty.Register("FontStretch", typeof(FontStretch?), typeof(LocationView), new PropertyMetadata(default(FontStretch?)));
        public static readonly DependencyProperty TextDecorationsProperty = DependencyProperty.Register("TextDecorations", typeof(TextDecorationCollection), typeof(LocationView), new PropertyMetadata(default(TextDecorationCollection)));
        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register("FontSize", typeof(double?), typeof(LocationView), new PropertyMetadata(default(double?)));

        public Location Location
        {
            get { return (Location)GetValue(LocationProperty); }
            set { SetValue(LocationProperty, value); }
        }

        public UserTagCollection Tags
        {
            get { return (UserTagCollection)GetValue(TagsProperty); }
            set { SetValue(TagsProperty, value); }
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LocationView view = (LocationView)d;

            if (e.Property == LocationProperty)
                view.OnLocationChanged((Location)e.OldValue, (Location)e.NewValue);
            else if (e.Property == TagsProperty)
                view.OnTagsChanged((UserTagCollection)e.OldValue, (UserTagCollection)e.NewValue);
            else
                throw new NotImplementedException();
        }

        private void OnLocationChanged(Location oldValue, Location newValue)
        {
            Tags = newValue.Tags;
            Tags.CollectionChanged += OnTagsCollectionChanged;
        }

        private void OnTagsChanged(UserTagCollection oldValue, UserTagCollection newValue)
        {
            Location.Tags = newValue;
            ConstructFontDescriptor();
        }

        public Brush BackgroundBrush
        {
            get { return (Brush)GetValue(BackgroundBrushProperty); }
            set { SetValue(BackgroundBrushProperty, value); }
        }

        public Brush ForegroundBrush
        {
            get { return (Brush)GetValue(ForegroundBrushProperty); }
            set { SetValue(ForegroundBrushProperty, value); }
        }

        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        public FontWeight? FontWeight
        {
            get { return (FontWeight?)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }

        public FontStyle? FontStyle
        {
            get { return (FontStyle?)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        public FontStretch? FontStretch
        {
            get { return (FontStretch?)GetValue(FontStretchProperty); }
            set { SetValue(FontStretchProperty, value); }
        }

        public TextDecorationCollection TextDecorations
        {
            get { return (TextDecorationCollection)GetValue(TextDecorationsProperty); }
            set { SetValue(TextDecorationsProperty, value); }
        }

        public double? FontSize
        {
            get { return (double?)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        private void ConstructFontDescriptor()
        {
            Brush background = null;
            Brush foreground = null;
            FontFamily fontFamily = null;
            FontWeight? fontWeight = null;
            FontStyle? fontStyle = null;
            FontStretch? fontStretch = null;
            TextDecorationCollection textDecorations = null;
            double? fontSize = null;

            foreach (UserTag tag in Tags.Order())
            {
                if (background == null && tag.Background != Colors.Transparent)
                    background = new SolidColorBrush(tag.Background);
                if (foreground == null && tag.Foreground != Colors.Transparent)
                    foreground = new SolidColorBrush(tag.Foreground);

                FontDescriptor font = tag.Font;
                if (font != null && !font.IsDefault)
                {
                    if (fontFamily == null)
                        fontFamily = font.FontFamily;
                    if (fontWeight == null)
                        fontWeight = font.FontWeight;
                    if (fontStyle == null)
                        fontStyle = font.FontStyle;
                    if (fontStretch == null)
                        fontStretch = font.FontStretch;
                    if (textDecorations == null)
                        textDecorations = font.TextDecorations;
                    if (fontSize == null)
                        fontSize = font.FontSize;
                }
            }

            BackgroundBrush = background ?? Brushes.Transparent;
            ForegroundBrush = foreground ?? Brushes.Black;
            FontFamily = fontFamily ?? new FontFamily();
            FontWeight = fontWeight ?? new FontWeight();
            FontStyle = fontStyle ?? new FontStyle();
            FontStretch = fontStretch ?? new FontStretch();
            TextDecorations = textDecorations ?? new TextDecorationCollection();
            FontSize = fontSize ?? 12;
        }
    }
}