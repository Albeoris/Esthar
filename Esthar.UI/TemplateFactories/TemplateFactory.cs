using System;
using Esthar.UI.MarkupExtensions;

namespace Esthar.UI
{
    public class TemplateFactory : BindingDecoratorBase
    {
        public TemplateFactoryType Template { get; set; }
        public string BindingPath { get; set; }
        public bool HasFontProperties { get; set; }

        public TemplateFactory()
        {
        }

        public TemplateFactory(string path)
        {
            BindingPath = path;
        }

        public override object ProvideValue(IServiceProvider provider)
        {
            if (Converter == null)
            {
                Converter = TemplateFactories.Get(this);
                ConverterParameter = BindingPath;
            }

            return base.ProvideValue(provider);
        }
    }
}