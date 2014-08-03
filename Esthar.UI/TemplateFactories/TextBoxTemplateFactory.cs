namespace Esthar.UI
{
    public sealed class TextBoxTemplateFactory : TemplateFactoryBase
    {
        public TextBoxTemplateFactory()
            : base(TemplateFactoryType.TextBox)
        {
        }

        protected override string GetTemplateContent(string propertyName)
        {
            return "<TextBox Text='{Binding " + propertyName + ", Mode=TwoWay}' BorderBrush='Transparent' Background='Transparent'/>";
        }
    }
}