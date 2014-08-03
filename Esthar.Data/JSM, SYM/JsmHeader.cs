namespace Esthar.Data
{
    public struct JsmHeader {
        public byte countLines;
        public byte countDoors;
        public byte countBackgrounds;
        public byte countOthers;
        public ushort section1;
        public ushort section2;
    }
}