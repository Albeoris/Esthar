using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Esthar.Core;

namespace Esthar.Data
{
    [Serializable]
    public sealed class ArchiveDirectoryEntry : ArchiveEntryBase
    {
        public Dictionary<string, ArchiveEntryBase> Childs { get; private set; }

        public ArchiveDirectoryEntry(string name)
            : base(name, ArchiveEntryType.Directory)
        {
            Childs = new Dictionary<string, ArchiveEntryBase>(PathComparer.Instance.Value);
        }

        public T GetChildEntry<T>(string path) where T : ArchiveEntryBase
        {
            T result = FindChildEntry<T>(path);
            if (result == null)
                throw Exceptions.CreateArgumentException("path", "Entry '{0}' is not found.", path);

            return result;
        }

        public T FindChildEntry<T>(string path) where T : ArchiveEntryBase
        {
            Exceptions.CheckArgumentNullOrEmprty(path, "path");

            string[] partNames = path.ToLowerInvariant().Split(Path.DirectorySeparatorChar);

            ArchiveDirectoryEntry dirNode = this;
            for (int i = 1; i < partNames.Length - 1; i++)
            {
                ArchiveDirectoryEntry child = dirNode.Childs.Values.OfType<ArchiveDirectoryEntry>().FirstOrDefault(c => c.Name == partNames[i]);
                if (child != null)
                    dirNode = child;
            }

            return (T)dirNode.Childs.TryGetValue(partNames.Last());
        }

        public void AddEntry(string path, ArchiveEntryBase entry)
        {
            Exceptions.CheckArgumentNullOrEmprty(path, "path");
            Exceptions.CheckArgumentNull(entry, "entry");

            if (ParentDirectory != null)
                throw new Exception("Узел не является корневым.");

            string[] partNames = path.ToLowerInvariant().Split(Path.DirectorySeparatorChar);
            if (partNames.Length < 2)
                throw Exceptions.CreateException("Заданный путь не содержит корневого узла или имени файла: '{1}'", Name, path);

            if (partNames.First() != Name)
                throw Exceptions.CreateException("Узел '{0}' не явяется корневым для заданного пути: '{1}'", Name, path);

            ArchiveDirectoryEntry dirNode = this;
            for (int i = 1; i < partNames.Length - 1; i++)
                dirNode = dirNode.EnsureDirectoryExists(partNames[i]);

            dirNode.AddEntry(entry);
        }

        private void AddEntry(ArchiveEntryBase entry)
        {
            Exceptions.CheckArgumentNull(entry, "entry");

            entry.ParentDirectory = this;
            Childs.Add(entry.Name, entry);
        }

        private ArchiveDirectoryEntry EnsureDirectoryExists(string partName)
        {
            ArchiveEntryBase dirEntry;

            if (!Childs.TryGetValue(partName, out dirEntry))
            {
                dirEntry = new ArchiveDirectoryEntry(partName) { ParentDirectory = this };
                Childs[partName] = dirEntry;
            }

            return (ArchiveDirectoryEntry)dirEntry;
        }
    }
}