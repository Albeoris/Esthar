using System.Linq;
using Esthar.Core;

namespace Esthar.Data
{
    public static class Archives
    {
        public static ArchiveDirectoryEntry Root { get; private set; }
        public static ArchiveInformation[] Infos { get; private set; }

        public static T GetEntry<T>(string path) where T : ArchiveEntryBase
        {
            Exceptions.CheckArgumentNullOrEmprty(path, "path");

            return Root.GetChildEntry<T>(path);
        }

        public static T FindEntry<T>(string path) where T : ArchiveEntryBase
        {
            Exceptions.CheckArgumentNullOrEmprty(path, "path");

            return Root.FindChildEntry<T>(path);
        }

        public static void Initialize(ArchiveInformation[] infos)
        {
            ArchiveDirectoryEntry root = new ArchiveDirectoryEntry("c:");

            foreach (ArchiveInformation info in infos)
                foreach (ArchiveFileEntry file in info.RootArchive.Expand())
                {
                    if (file.ParentArchive == null)
                        continue;

                    root.AddEntry(file.GetFullPath(), file);
                }

            while (root.Childs.Count == 1)
            {
                ArchiveDirectoryEntry child = root.Childs.First().Value as ArchiveDirectoryEntry;
                if (child == null)
                    break;

                root = child;
            }

            Infos = infos;
            Root = root;
        }

        public static ArchiveInformation GetInfo(ArchiveName name)
        {
            return Infos.First(i => PathComparer.Instance.Value.Equals(i.RootArchive.Name, name.ToString()));
        }
    }
}