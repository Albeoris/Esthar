namespace Esthar.UI
{
    public sealed class TextBlockTemplateFactory : TemplateFactoryBase
    {
        public TextBlockTemplateFactory()
            : base(TemplateFactoryType.TextBlock)
        {
        }

        protected override string GetTemplateContent(string propertyName)
        {
            return "<TextBlock Text='{Binding " + propertyName + ", Mode=OneWay}' />";
        }
    }
}