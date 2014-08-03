using System;

namespace Esthar.UI
{
    public static class TemplateFactories
    {
        public static TextBlockTemplateFactory TextBlock
        {
            get { return TextBlockTemplateFactoryInstance.Value; }
        }

        public static FontedTextBlockTemplateFactory FontedTextBlock
        {
            get { return FontedTextBlockTemplateFactoryInstance.Value; }
        }

        public static TextBoxTemplateFactory TextBox
        {
            get { return TextBoxTemplateFactoryInstance.Value; }
        }

        public static FontedTextBoxTemplateFactory FontedTextBox
        {
            get { return FontedTextBoxTemplateFactoryInstance.Value; }
        }

        public static CheckBoxTemplateFactory CheckBox
        {
            get { return CheckBoxTemplateFactoryInstance.Value; }
        }

        public static ColorPickerTemplateFactory ColorPicker
        {
            get { return ColorPickerTemplateFactoryInstance.Value; }
        }

        public static TemplateFactoryBase Get(TemplateFactory spec)
        {
            switch (spec.Template)
            {
                case TemplateFactoryType.TextBlock:
                    return GetTextBlock(spec);
                case TemplateFactoryType.TextBox:
                    return GetTextBox(spec);
                case TemplateFactoryType.CheckBox:
                    return GetCheckBox(spec);
                case TemplateFactoryType.ColorPicker:
                    return GetColorPicker(spec);
                default:
                    throw new NotImplementedException();
            }
        }

        private static TemplateFactoryBase GetTextBlock(TemplateFactory spec)
        {
            return spec.HasFontProperties ? (TemplateFactoryBase)FontedTextBlock : TextBlock;
        }

        private static TemplateFactoryBase GetTextBox(TemplateFactory spec)
        {
            return spec.HasFontProperties ? (TemplateFactoryBase)FontedTextBox : TextBox;
        }

        private static TemplateFactoryBase GetCheckBox(TemplateFactory spec)
        {
            return CheckBox;
        }

        private static TemplateFactoryBase GetColorPicker(TemplateFactory spec)
        {
            return ColorPicker;
        }

        private static readonly Lazy<TextBlockTemplateFactory> TextBlockTemplateFactoryInstance = new Lazy<TextBlockTemplateFactory>();
        private static readonly Lazy<FontedTextBlockTemplateFactory> FontedTextBlockTemplateFactoryInstance = new Lazy<FontedTextBlockTemplateFactory>();

        private static readonly Lazy<TextBoxTemplateFactory> TextBoxTemplateFactoryInstance = new Lazy<TextBoxTemplateFactory>();
        private static readonly Lazy<FontedTextBoxTemplateFactory> FontedTextBoxTemplateFactoryInstance = new Lazy<FontedTextBoxTemplateFactory>();

        private static readonly Lazy<CheckBoxTemplateFactory> CheckBoxTemplateFactoryInstance = new Lazy<CheckBoxTemplateFactory>();
        private static readonly Lazy<ColorPickerTemplateFactory> ColorPickerTemplateFactoryInstance = new Lazy<ColorPickerTemplateFactory>();
    }
}