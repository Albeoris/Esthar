using System.Runtime.InteropServices;

namespace Esthar.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct KernellS14GFCharacterAbility
    {
        public const int TextSection = 44;

        public ushort NameOffset;
        public ushort DescriptionOffset;

        public uint Unknown;

        //                      0000 0700 3C001400
        //1300 1A00 78002800    2600 2D00 F0005000
        //3900 4100 3C011400    4E00 5600 78012800
        //6300 6B00 F0013C00    7800 8000 3C021400
        //8E00 9600 78022800    A400 AC00 F0023C00
        //BA00 C200 3C031400    D600 DE00 78032800
        //F200 FA00 F0033C00    0E01 1601 3C041400
        //2301 2B01 78042800    3801 4001 F0043C00
        //4D01 5501 96051400    6201 6A01 C8052800
        //7701 7F01 96061E00    8D01 9601 C8083200
    }
}