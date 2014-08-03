using System;

namespace Esthar.Data.Transform
{
    public static class AsmModuleFactory
    {
        public static AsmModule Create(Data.AsmModuleType type)
        {
            switch (type)
            {
                case Data.AsmModuleType.Door:
                    return new AsmDoor();
                case Data.AsmModuleType.Area:
                    return new AsmArea();
                case Data.AsmModuleType.Module:
                    return new AsmModule(AsmModuleType.Module);
                case Data.AsmModuleType.Object:
                    return new AsmObject();
            }

            throw new NotImplementedException();
        }
    }
}