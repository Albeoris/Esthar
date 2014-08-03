namespace Esthar.UI
{
    public sealed class FontedTextBlockTemplateFactory : TemplateFactoryBase
    {
        public FontedTextBlockTemplateFactory()
            : base(TemplateFactoryType.TextBlock)
        {
        }

        protected override string GetTemplateContent(string propertyName)
        {
            return "<TextBlock " +
                   "Text='{Binding " + propertyName + ", Mode=OneWay}' " +
                   "FontFamily='{Binding Path=FontFamily}' " +
                   "FontWeight='{Binding FontWeight}' " +
                   "FontStyle='{Binding FontStyle}' " +
                   "FontStretch='{Binding FontStretch}' " +
                   "TextDecorations='{Binding TextDecorations}' " +
                   "FontSize='{Binding FontSize}' " +
                   "Foreground='{Binding ForegroundBrush}' " +
                   "Background='{Binding BackgroundBrush}' />";
        }
    }
}