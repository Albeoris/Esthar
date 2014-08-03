using System.Runtime.InteropServices;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public sealed class MimTile
    {
        [FieldOffset(0)] public MimSimpleTile Simple;
        [FieldOffset(0)] public MimLayeredTile Layered;

        public bool IsEndOfFile()
        {
            return Simple.X == 0x7fff;
        }
    }
}