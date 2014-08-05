using System;
using System.Xml;
using Esthar.Core;

namespace Esthar.Data.Transform
{
    public sealed class LocalizableString
    {
        public string Original { get; private set; }
        public string Current { get; set; }

        public int Index { get; set; }
        public int Order { get; set; }
        public bool IsIndent { get; set; }

        public LocalizableString(string original, string current)
        {
            Order = -1;
            IsIndent = false;
            Original = Exceptions.CheckArgumentNull(original, "original");
            Current = Exceptions.CheckArgumentNull(current, "current");
        }

        public static LocalizableStrings FromXml(XmlElement node)
        {
            LocalizableStrings result = new LocalizableStrings(node.ChildNodes.Count);
            foreach (XmlElement child in node.ChildNodes)
            {
                int? order = child.FindInt32("Order");
                bool? isIndent = child.FindBoolean("IsIndent");
                string orig = child.GetString("Original");
                string curr = child.GetString("Current");
                
                LocalizableString str = new LocalizableString(orig, curr);
                if (order != null) str.Order = order.Value;
                if (isIndent != null) str.IsIndent = isIndent.Value;
                
                result.Add(str);
            }
            return result;
        }

        public static string ResolveNewLine(string text)
        {
            return text.Replace(FF8TextTag.LineSeparator[0], Environment.NewLine).Replace(FF8TextTag.PageSeparator[0], Environment.NewLine + FF8TextTag.PageSeparator[0] + Environment.NewLine);
        }

        public static string ReturnNewLine(string text)
        {
            return text.TrimEnd().Replace(Environment.NewLine + FF8TextTag.PageSeparator[0] + Environment.NewLine, FF8TextTag.PageSeparator[0]).Replace(Environment.NewLine, FF8TextTag.LineSeparator[0]);
        }

        public bool MatchFilter(string text)
        {
            if (string.IsNullOrEmpty(text))
                return true;

            if (Current.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) >= 0)
                return true;

            if (Original.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) >= 0)
                return true;

            return false;
        }
    }
}