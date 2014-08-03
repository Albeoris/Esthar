using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xml;

namespace Esthar.UI
{
    public abstract class TemplateFactoryBase : IValueConverter
    {
        private readonly TemplateFactoryType _type;

        protected TemplateFactoryBase(TemplateFactoryType type)
        {
            _type = type;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (DesignerProperties.GetIsInDesignMode(Dummy.DependencyObject))
                return null;

            string propertyName = parameter as string;
            if (string.IsNullOrEmpty(propertyName))
                return null;

            string key = _type + "TemplateBy" + propertyName;
            DataTemplate result = (DataTemplate)Application.Current.TryFindResource(key);
            if (result != null)
                return result;

            StringBuilder templateXaml = new StringBuilder();
            templateXaml.Append("<DataTemplate ");
            templateXaml.Append("xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' ");
            templateXaml.Append("xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' ");
            foreach (string str in GetNamespaces())
            {
                templateXaml.Append(str);
                templateXaml.Append(' ');
            }
            templateXaml.Append("x:Key='").Append(key).Append("'>");
            templateXaml.Append(GetTemplateContent(propertyName));
            templateXaml.Append("</DataTemplate>");

            using (StringReader stringReader = new StringReader(templateXaml.ToString()))
            using (XmlReader xmlReader = XmlReader.Create(stringReader))
                result = (DataTemplate)XamlReader.Load(xmlReader);

            PostProcess(result);
            Application.Current.Resources.Add(key, result);
            return result;
        }

        protected virtual void PostProcess(DataTemplate result)
        {
        }

        protected virtual IEnumerable<string> GetNamespaces()
        {
            return new string[0];
        }
        
        protected abstract string GetTemplateContent(string propertyName);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}