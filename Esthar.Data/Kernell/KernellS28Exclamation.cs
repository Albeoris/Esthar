using System.Runtime.InteropServices;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct KernellS28Exclamation
    {
        public const int TextSection = 54;

        public ushort NameOffset;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)] public byte[] Unknown;

        //0000 1E 08 00 00 00 00 00 00 00 00
        //0F00 1E 10 00 00 00 00 00 00 00 00
        //1C00 1E 10 05 56 00 00 7E 00 00 00
        //2800 1F 01 00 00 00 00 40 00 00 00
        //3700 1F 00 00 00 00 00 04 00 00 00
        //4700 1F 01 00 00 00 00 08 00 00 00
        //5A00 1F 02 00 00 00 00 02 00 00 00
        //6A00 1F 08 00 00 00 00 02 00 00 00
        //7B00 1F 0C 05 02 00 00 1A 00 00 00
        //9700 1E 00 00 00 00 00 00 00 00 00
        //A000 1E 10 05 56 00 00 7E 00 01 00
        //B000 1E 10 05 56 00 00 7E 00 02 00
        //BF00 1E 10 05 56 00 00 7E 00 04 00
        //CB00 1E 10 05 56 00 00 7E 00 08 00
        //DD00 1E 10 05 56 00 00 7E 00 10 00
        //EF00 1E 10 05 56 00 00 7E 00 00 0A            
    }
}