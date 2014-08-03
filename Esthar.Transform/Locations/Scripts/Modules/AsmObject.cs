using System.Collections.Generic;
using System.Linq;

namespace Esthar.Data.Transform
{
    public class AsmObject : AsmModule
    {
        public AsmEvent Talk;
        public AsmEvent Push;

        public AsmObject()
            : base(AsmModuleType.Object)
        {
        }

        public AsmCharacterId CharacterId = AsmCharacterId.None;

        public override int Count
        {
            get
            {
                int count = base.Count;
                if (Talk != null) count++;
                if (Push != null) count++;
                return count;
            }
        }

        public override IEnumerable<AsmEvent> GetOrderedEvents()
        {
            yield return Construct;
            yield return Initialize;

            if (Talk == null) yield break;
            yield return Talk;

            if (Push == null) yield break;
            yield return Push;

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
                    Talk = script;
                    break;
                case 3:
                    Push = script;
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
                    return Talk;
                case 3:
                    return Push;
                default:
                    return GetScriptByLabel(label);
            }
        }
    }
}