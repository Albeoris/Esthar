using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using Esthar.Core;

namespace Esthar.Data
{
    [Serializable]
    public abstract class ArchiveEntryBase
    {
        public string Name { get; internal set; }
        public ArchiveEntryType Type { get; private set; }

        public ArchiveArchiveEntry ParentArchive { get; internal set; }
        public ArchiveDirectoryEntry ParentDirectory { get; internal set; }

        protected ArchiveEntryBase(string name, ArchiveEntryType type)
        {
            Exceptions.CheckArgumentNullOrEmprty(name, "name");

            Name = name;
            Type = type;
        }

        public string GetFullPath()
        {
            var list = new List<string>(12) { Name };

            ArchiveDirectoryEntry dir = ParentDirectory;
            while (dir != null)
            {
                list.Add(dir.Name);
                dir = dir.ParentDirectory;
            }

            list.Reverse();
            list[0] += "\\";
            return Path.Combine(list.ToArray());
        }
    }
}