using System.Runtime.InteropServices;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct KernellS16GFAbility
    {
        public const int TextSection = 46;

        public ushort NameOffset;
        public ushort DescriptionOffset;

        public uint Unknown;

        //0000 0600 C8010000    1800 2200 28100000
        //3C00 4500 1E040000    5D00 6600 64080000
        //7200 7C00 FA020000                           
    }
}