using System.Text;

namespace Esthar.UI
{
    public sealed class CheckBoxTemplateFactory : TemplateFactoryBase
    {
        public CheckBoxTemplateFactory()
            : base(TemplateFactoryType.CheckBox)
        {
        }

        protected override string GetTemplateContent(string propertyName)
        {
            return "<CheckBox IsChecked='{Binding " + propertyName + ", Mode=TwoWay}'>" +
                   "<CheckBox.LayoutTransform><ScaleTransform ScaleX='2' ScaleY='2' /></CheckBox.LayoutTransform>" +
                   "</CheckBox>";
        }
    }
}