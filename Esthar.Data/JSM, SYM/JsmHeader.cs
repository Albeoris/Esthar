namespace Esthar.Data
{
    public struct JsmHeader
    {
        public byte CountAreas;
        public byte CountDoors;
        public byte CountModules;
        public byte CountObjects;
        public ushort ScriptsOffset;
        public ushort OperationsOffset;

        public void IncrementCount(JsmModuleType type)
        {
            switch (type)
            {
                case JsmModuleType.Area:
                    CountAreas++;
                    break;
                case JsmModuleType.Door:
                    CountDoors++;
                    break;
                case JsmModuleType.Module:
                    CountModules++;
                    break;
                case JsmModuleType.Object:
                    CountObjects++;
                    break;
            }
        }
    }
}