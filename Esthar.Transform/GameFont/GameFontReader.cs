using System.IO;
using System.Xml;
using Esthar.Core;
using Esthar.OpenGL;

namespace Esthar.Data.Transform
{
    public static class GameFontReader
    {
        public const string MainPath = @"c:\ff8\data\eng";
        public const string MenuPath = @"c:\ff8\data\eng\menu";
        public const string HiResDirectoryName = "hires";
        public const string HiResCharactersWidthsFileName = @"sysfnt.tdw";
        public const string HiResFontImage1FileName = @"sysfld00.TEX";
        public const string HiResFontImage2FileName = @"sysfld01.TEX";

        public static GameFont FromDirectory(string dirPath)
        {
            string xmlPath = Path.Combine(dirPath, "Characters.xml");
            if (!File.Exists(xmlPath))
                return null;

            XmlElement node = XmlHelper.LoadDocument(xmlPath);

            byte[] fontWidths = new byte[node.ChildNodes.Count];
            for (int i = 0; i < fontWidths.Length; i++)
                fontWidths[i] = ((XmlElement)node.ChildNodes[i]).GetByte("Width");

            GameImage image = GameImageReader.FromDirectory(dirPath);
            return new GameFont(image, fontWidths);
        }

        public static GameFont FromGameData(ArchiveDirectoryEntry directory, string name)
        {
            ArchiveFileEntry tdwEntry = (ArchiveFileEntry)directory.Childs.TryGetValue(name + ".tdw");
            if (tdwEntry == null)
                return null;

            using (TdwFileReader tdwReader = new TdwFileReader(tdwEntry.OpenReadableContentStream()))
            using (TimFileReader timReader = tdwReader.TimReader)
            {
                if (timReader == null)
                    return null;

                GameImage image = GameImageReader.FromTim(timReader);
                return new GameFont(image, tdwReader.Table);
            }
        }

        public static GameFont HiResFromGameData()
        {
            using (DisposableStack disposables = new DisposableStack(2))
            {
                GameImage firstImage, secondImage;

                ArchiveDirectoryEntry mainDirectory = Archives.GetEntry<ArchiveDirectoryEntry>(MainPath);
                ArchiveDirectoryEntry menuDirectory = Archives.GetEntry<ArchiveDirectoryEntry>(MenuPath);
                ArchiveDirectoryEntry hiresDirectory = menuDirectory.GetChildEntry<ArchiveDirectoryEntry>(HiResDirectoryName);
                ArchiveFileEntry tdwEntry = mainDirectory.GetChildEntry<ArchiveFileEntry>(HiResCharactersWidthsFileName);
                ArchiveFileEntry image01 = hiresDirectory.GetChildEntry<ArchiveFileEntry>(HiResFontImage1FileName);
                ArchiveFileEntry image02 = hiresDirectory.GetChildEntry<ArchiveFileEntry>(HiResFontImage2FileName);

                byte[] widths;
                using (TdwFileReader tdwReader = new TdwFileReader(tdwEntry.OpenReadableContentStream()))
                    widths = tdwReader.Table;

                using (TexFileReader texReader = new TexFileReader(image01.OpenReadableContentStream()))
                {
                    firstImage = GameImageReader.FromTex(texReader);
                    disposables.Add(firstImage.Layer);
                }

                using (TexFileReader texReader = new TexFileReader(image02.OpenReadableContentStream()))
                    secondImage = disposables.Add(GameImageReader.FromTex(texReader));

                int x = firstImage.X;
                int y = firstImage.Y;
                GLTexture palettes = firstImage.Palettes;
                GLTexture layer = GLTextureFactory.HorizontalJoin(firstImage.Layer, secondImage.Layer);
                GameImage image = new GameImage(x, y, layer, palettes);

                return new GameFont(image, widths);
            }
        }
    }
}