using System.Windows;

namespace Esthar.UI
{
    public static class AP
    {
        public static readonly DependencyProperty TemplateBindingProperty = DependencyProperty.RegisterAttached("TemplateBinding", typeof(string), typeof(AP), new PropertyMetadata(default(string)));

        public static void SetTemplateBinding(DependencyObject element, string value)
        {
            element.SetValue(TemplateBindingProperty, value);
        }

        public static string GetTemplateBinding(DependencyObject element)
        {
            return (string)element.GetValue(TemplateBindingProperty);
        }
    }
}