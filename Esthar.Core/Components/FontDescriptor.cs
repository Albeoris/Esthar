using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Xml;

namespace Esthar.Core
{
    public sealed class FontDescriptor
    {
        public bool IsDefault { get; private set; }

        public FontFamily FontFamily { get; set; }
        public FontWeight FontWeight { get; set; }
        public FontStyle FontStyle { get; set; }
        public FontStretch FontStretch { get; set; }
        public TextDecorationCollection TextDecorations { get; set; }
        public double FontSize { get; set; }

        public static FontDescriptor Default
        {
            get { return DefaultInstance.Value; }
        }

        private static FontDescriptor CreateDefault()
        {
            return new FontDescriptor
            {
                IsDefault = true,

                FontFamily = new FontFamily(),
                FontWeight = new FontWeight(),
                FontStyle = new FontStyle(),
                FontStretch = new FontStretch(),
                TextDecorations = new TextDecorationCollection(),
                FontSize = 12
            };
        }

        private static readonly Lazy<FontDescriptor> DefaultInstance = new Lazy<FontDescriptor>(CreateDefault);

        public void Serialize(XmlElement node)
        {
            Exceptions.CheckArgumentNull(node, "node").RemoveAll();

            if (IsDefault)
            {
                node.SetBoolean("IsDefault", IsDefault);
                return;
            }

            node.SetString("Family", FontFamily.Source);
            node.SetInt32("Weight", FontWeight.ToOpenTypeWeight());
            node.SetInt32("Style", (FontStyleConverter)FontStyle);
            node.SetInt32("Stretch", FontStretch.ToOpenTypeStretch());
            node.SetDouble("Size", FontSize);

            if (!TextDecorations.IsNullOrEmpty())
            {
                XmlElement textDecorationsNode = node.CreateChildElement("TextDecorations");
                foreach (TextDecoration decoration in TextDecorations)
                    textDecorationsNode.CreateChildElement("Decoration").SetInt32("Location", (int)decoration.Location);
            }
        }

        public static FontDescriptor Deserialize(XmlElement node)
        {
            if (node.FindBoolean("IsDefault") == true)
                return Default;

            string fontFamily = node.FindString("Family");
            int? fontWight = node.FindInt32("Weight");
            int? fontStyle = node.FindInt32("Style");
            int? fontStretch = node.FindInt32("Stretch");
            double? fontSize = node.FindDouble("Size");

            TextDecorationCollection textDecorations = new TextDecorationCollection();
            XmlElement textDecorationsNode = node["TextDecorations"];
            if (textDecorationsNode != null)
            {
                foreach (int? location in textDecorationsNode.OfType<XmlElement>().Select(decorationNode => decorationNode.FindInt32("Location")).Where(location => location != null))
                {
                    switch ((TextDecorationLocation)location.Value)
                    {
                        case TextDecorationLocation.Baseline:
                            textDecorations.Add(System.Windows.TextDecorations.Baseline[0]);
                            break;
                        case TextDecorationLocation.OverLine:
                            textDecorations.Add(System.Windows.TextDecorations.OverLine[0]);
                            break;
                        case TextDecorationLocation.Strikethrough:
                            textDecorations.Add(System.Windows.TextDecorations.Strikethrough[0]);
                            break;
                        case TextDecorationLocation.Underline:
                            textDecorations.Add(System.Windows.TextDecorations.Underline[0]);
                            break;
                    }
                }
            }

            return new FontDescriptor
            {
                FontFamily = string.IsNullOrEmpty(fontFamily) ? new FontFamily() : new FontFamily(fontFamily),
                FontWeight = fontWight == null ? new FontWeight() : FontWeight.FromOpenTypeWeight(fontWight.Value),
                FontStyle = fontStyle == null ? new FontStyle() : (FontStyle)(FontStyleConverter)fontStyle.Value,
                FontStretch = fontStretch == null ? new FontStretch() : FontStretch.FromOpenTypeStretch(fontStretch.Value),
                FontSize = fontSize ?? 12,
                TextDecorations = textDecorations
            };
        }
    }
}