using System;
using System.Windows.Data;

namespace Esthar.UI.MarkupExtensions
{
    public class AncestorBinding : BindingDecoratorBase
    {
        public Type AncestorType { get; set; }
        public int AncestorLevel { get; set; }

        public AncestorBinding()
        {
            AncestorLevel = 1;
        }

        public AncestorBinding(string path)
            : base(path)
        {
            AncestorLevel = 1;
        }

        public override object ProvideValue(IServiceProvider provider)
        {
            if (RelativeSource == null)
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, AncestorType, AncestorLevel);

            return base.ProvideValue(provider);
        }
    }
}