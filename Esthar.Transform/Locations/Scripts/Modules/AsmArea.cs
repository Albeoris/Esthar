using System.Collections.Generic;
using System.Linq;

namespace Esthar.Data.Transform
{
    public class AsmArea : AsmModule
    {
        public AsmEvent Talk;
        public AsmEvent Push;
        private AsmEvent Across;
        private AsmEvent Touch;
        private AsmEvent TouchOn;
        private AsmEvent TouchOff;

        public AsmArea()
            : base(JsmModuleType.Area)
        {
        }

        public override int Count
        {
            get
            {
                int count = base.Count;
                if (Talk != null) count++;
                if (Push != null) count++;
                if (Across != null) count++;
                if (Touch != null) count++;
                if (TouchOn != null) count++;
                if (TouchOff != null) count++;
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
            
            if (Across == null) yield break;
            yield return Across;
            
            if (Touch == null) yield break;
            yield return Touch;
            
            if (TouchOn == null) yield break;
            yield return TouchOn;
            
            if (TouchOff == null) yield break;
            yield return TouchOff;
            
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
                case 4:
                    Across = script;
                    break;
                case 5:
                    Touch = script;
                    break;
                case 6:
                    TouchOn = script;
                    break;
                case 7:
                    TouchOff = script;
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
                case 4:
                    return Across;
                case 5:
                    return Touch;
                case 6:
                    return TouchOn;
                case 7:
                    return TouchOff;
                default:
                    return GetScriptByLabel(label);
            }
        }
    }
}