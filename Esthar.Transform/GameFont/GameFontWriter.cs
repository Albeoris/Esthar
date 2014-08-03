using System.IO;
using System.Linq;
using System.Xml;
using Esthar.Core;
using Esthar.OpenGL;

namespace Esthar.Data.Transform
{
    public static class GameFontWriter
    {
        public static void ToDirectory(GameFont extraFont, string dirPath)
        {
            string xmlPath = FileCommander.EnsureDirectoryExists(dirPath, "Characters.xml");

            XmlElement characters = XmlHelper.CreateDocument("Characters");
            foreach (byte width in extraFont.CharactersWidths)
                characters.CreateChildElement("Character").SetByte("Width", width);
            characters.GetOwnerDocument().Save(xmlPath);

            GameImageWriter.ToDirectory(extraFont.CharactersImage, dirPath);
        }

        public static void HiResToGameData(GameFont gameFont)
        {
            using (DisposableStack disposables = new DisposableStack(2))
            {
                GLTexture leftTexture;
                GLTexture rightTexture;
                GLTextureFactory.HorizontalSplit(gameFont.CharactersImage.Layer, out leftTexture, out rightTexture);
                disposables.Add(leftTexture);
                disposables.Add(rightTexture);

                ArchiveDirectoryEntry mainDirectory = Archives.GetEntry<ArchiveDirectoryEntry>(GameFontReader.MainPath);
                ArchiveDirectoryEntry menuDirectory = Archives.GetEntry<ArchiveDirectoryEntry>(GameFontReader.MenuPath);
                ArchiveDirectoryEntry hiresDirectory = menuDirectory.GetChildEntry<ArchiveDirectoryEntry>(GameFontReader.HiResDirectoryName);
                ArchiveFileEntry tdwEntry = mainDirectory.GetChildEntry<ArchiveFileEntry>(GameFontReader.HiResCharactersWidthsFileName);
                ArchiveFileEntry image01 = hiresDirectory.GetChildEntry<ArchiveFileEntry>(GameFontReader.HiResFontImage1FileName);
                ArchiveFileEntry image02 = hiresDirectory.GetChildEntry<ArchiveFileEntry>(GameFontReader.HiResFontImage2FileName);

                using (Stream output = tdwEntry.OpenWritableCapacityStream())
                using (TdwFileWriter tdwWriter = new TdwFileWriter(output))
                {
                    tdwWriter.WriteFontCharactersWidths(gameFont.CharactersWidths);
                    tdwEntry.UpdateMetrics((int)output.Position, tdwEntry.ContentOffset, Compression.None);
                }

                using (Stream output = image01.OpenWritableCapacityStream())
                using (TexFileWriter texWriter = new TexFileWriter(output))
                {
                    texWriter.WriteImage(leftTexture, gameFont.CharactersImage.Palettes);
                    image01.UpdateMetrics((int)output.Position, image01.ContentOffset, Compression.None);
                }

                using (Stream output = image02.OpenWritableCapacityStream())
                using (TexFileWriter texWriter = new TexFileWriter(output))
                {
                    texWriter.WriteImage(rightTexture, gameFont.CharactersImage.Palettes);
                    image02.UpdateMetrics((int)output.Position, image02.ContentOffset, Compression.None);
                }

                Archives.GetInfo(ArchiveName.Main).Update();
                Archives.GetInfo(ArchiveName.Menu).Update();
            }
        }
    }
}