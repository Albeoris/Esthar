using System;
using System.Xml;

namespace Esthar.Core
{
    public static class XmlDocumentExm
    {
        public static XmlElement GetDocumentElement(this XmlDocument self)
        {
            Exceptions.CheckArgumentNull(self, "self");

            XmlElement element = self.DocumentElement;
            if (element == null)
                throw new ArgumentException("XmlElement �� ������.", "self");

            return element;
        }
    }
}