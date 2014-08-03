using System.Collections.Generic;
using System.Linq;

namespace Esthar.Data.Transform
{
    public class AsmDoor : AsmModule
    {
        public AsmEvent Open;
        public AsmEvent Close;
        public AsmEvent On;
        public AsmEvent Off;

        public AsmDoor()
            : base(AsmModuleType.Door)
        {
        }

        public override int Count
        {
            get
            {
                int count = base.Count;
                if (Open != null) count++;
                if (Close != null) count++;
                if (On != null) count++;
                if (Off != null) count++;
                return count;
            }
        }

        public override IEnumerable<AsmEvent> GetOrderedEvents()
        {
            yield return Construct;
            yield return Initialize;
            
            if (Open == null) yield break;
            yield return Open;
            
            if (Close == null) yield break;
            yield return Close;
            
            if (On == null) yield break;
            yield return On;
            
            if (Off == null) yield break;
            yield return Off;
            
            foreach (AsmEvent script in Scripts.Values.OrderBy(s => s.Label))
                yield return script;
        }

        public override void SetEventByIndex(int index, AsmEvent script)
        {
            switch (index)
            {
                case 0:
                    Construct = script;
                    break;
                case 1:
                    Initialize = script;
                    break;
                case 2:
                    Open = script;
                    break;
                case 3:
                    Close = script;
                    break;
                case 4:
                    On = script;
                    break;
                case 5:
                    Off = script;
                    break;
                default:
                    Scripts[script.Label] = script;
                    break;
            }
        }

        public override AsmEvent GetEventByLabel(ushort label)
        {
            switch (label - Label)
            {
                case 0:
                    return Construct;
                case 1:
                    return Initialize;
                case 2:
                    return Open;
                case 3:
                    return Close;
                case 4:
                    return On;
                case 5:
                    return Off;
                default:
                    return GetScriptByLabel(label);
            }
        }
    }
}