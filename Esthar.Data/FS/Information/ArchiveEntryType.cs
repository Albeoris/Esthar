using System;

namespace Esthar.Data
{
    [Serializable]
    public enum ArchiveEntryType
    {
        Unknown = 0,
        Archive = 1,
        Directory = 2,
        File = 3
    }
}