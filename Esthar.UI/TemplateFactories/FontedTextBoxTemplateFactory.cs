namespace Esthar.UI
{
    public sealed class FontedTextBoxTemplateFactory : TemplateFactoryBase
    {
        public FontedTextBoxTemplateFactory()
            : base(TemplateFactoryType.TextBox)
        {
        }

        protected override string GetTemplateContent(string propertyName)
        {
            return "<TextBox " +
                   "Text='{Binding " + propertyName + ", Mode=TwoWay}' BorderBrush='Transparent' " +
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