using System.Collections.Generic;

namespace Esthar.UI
{
    public sealed class ColorPickerTemplateFactory : TemplateFactoryBase
    {
        public ColorPickerTemplateFactory()
            : base(TemplateFactoryType.ColorPicker)
        {
        }

        protected override IEnumerable<string> GetNamespaces()
        {
            return new[] { "xmlns:xctk='http://schemas.xceed.com/wpf/xaml/toolkit'" };
        }

        protected override string GetTemplateContent(string propertyName)
        {
            return "<xctk:ColorPicker SelectedColor='{Binding " + propertyName + ", Mode=TwoWay}' ColorMode='ColorCanvas' ShowStandardColors='False' ShowAvailableColors='False'/>";
        }
    }
}