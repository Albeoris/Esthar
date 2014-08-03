using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Esthar.Core;

namespace Esthar.Data.Transform
{
    public static class Glossary
    {
        private const string SourcePath = @"c:\ff8\data\eng\menu";
        private const string AreaNamesFileName = @"areames.dc1";
        private static readonly string GlossaryPath = Path.Combine(Options.WorkingDirectory, "Glossary.xml");

        private static Lazy<XmlElement> _glossaryNode;
        private static Lazy<LocalizableStrings> _areaNames;

        static Glossary()
        {
            _glossaryNode = new Lazy<XmlElement>(() => XmlHelper.TryLoadDocument(GlossaryPath));
            _areaNames = new Lazy<LocalizableStrings>(LoadAreaNames);
        }

        public static LocalizableStrings AreaNames
        {
            get { return _areaNames.Value; }
        }

        private static LocalizableStrings LoadAreaNames()
        {
            lock (_glossaryNode)
            {
                if (_glossaryNode.Value != null)
                {
                    XmlElement node = _glossaryNode.Value.GetChildElement("AreaNames");
                    return LocalizableString.FromXml(node);
                }

                ArchiveDirectoryEntry dir = Archives.GetEntry<ArchiveDirectoryEntry>(SourcePath);
                ArchiveFileEntry entry1 = dir.GetChildEntry<ArchiveFileEntry>(AreaNamesFileName);

                LocalizableStrings result = new LocalizableStrings(256);

                using (DictionaryFileReader dicReader = new DictionaryFileReader(entry1.OpenReadableContentStream()))
                    foreach (string title in dicReader.Titles)
                        result.Add(new LocalizableString(title, title));

                return result;
            }
        }
    }
}