using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public sealed class CraftingBinEntry
    {
        public const int StructSize = 8;

        public ushort MessageOffset;
        public byte TargetItemsCount;
        public ushort Maximum = 256;
        public byte SourceItemId;
        public byte SourceItemsCount;
        public byte TargetItemId;
    }
}