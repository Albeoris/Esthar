using System;

namespace Esthar.Data.Transform
{
    public static class AsmModuleFactory
    {
        public static AsmModule Create(Data.JsmModuleType type)
        {
            switch (type)
            {
                case Data.JsmModuleType.Door:
                    return new AsmDoor();
                case Data.JsmModuleType.Area:
                    return new AsmArea();
                case Data.JsmModuleType.Module:
                    return new AsmModule(JsmModuleType.Module);
                case Data.JsmModuleType.Object:
                    return new AsmObject();
            }

            throw new NotImplementedException();
        }
    }
}