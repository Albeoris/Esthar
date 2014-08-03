using System.Windows.Data;
using Esthar.UI.MarkupExtensions;

namespace Esthar.UI.MarkupExtensions
{
    public class SelfBinding : BindingDecoratorBase
    {
        public SelfBinding()
        {
            RelativeSource = new RelativeSource(RelativeSourceMode.Self);
        }

        public SelfBinding(string path)
            : base(path)
        {
            RelativeSource = new RelativeSource(RelativeSourceMode.Self);
        }
    }
}