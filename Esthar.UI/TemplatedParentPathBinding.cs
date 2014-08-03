using System.Windows;
using System.Windows.Data;
using Esthar.UI.MarkupExtensions;

namespace Esthar.UI
{
    public class TemplatedParentPathBinding : BindingDecoratorBase
    {
        public TemplatedParentPathBinding()
        {
        }

        public override object ProvideValue(System.IServiceProvider provider)
        {
            Binding binding = new Binding
            {
                Mode = BindingMode.OneTime,
                Source = new RelativeSource(RelativeSourceMode.TemplatedParent),
                Path = new PropertyPath(AP.TemplateBindingProperty)
            };

            string path;
            lock (DummyInstance)
            {
                BindingOperations.SetBinding(DummyInstance, Dummy.ValueProperty, binding);
                path = (string)DummyInstance.GetValue(Dummy.ValueProperty);
                BindingOperations.ClearAllBindings(DummyInstance);
            }

            binding = new Binding { Path = new PropertyPath(path) };
            return binding.ProvideValue(provider);
        }

        private static readonly Dummy DummyInstance = new Dummy();

        private class Dummy : DependencyObject
        {
            public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(Dummy), new UIPropertyMetadata(null));
        }
    }
}