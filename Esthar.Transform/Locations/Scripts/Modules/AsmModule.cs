using System.Collections.Generic;
using System.Linq;

namespace Esthar.Data.Transform
{
    public class AsmModule
    {
        public ushort Index;
        public ushort ExecutionOrder;
        public ushort Label;
        public string Title;

        public AsmModule PreviousModule, NextModule;

        public readonly AsmModuleType Type;
        public AsmEvent Construct;
        public AsmEvent Initialize;
        public readonly Dictionary<ushort, AsmEvent> Scripts = new Dictionary<ushort, AsmEvent>();

        public AsmCollection ParentCollection;

        public AsmModule(AsmModuleType type)
        {
            Type = type;
        }

        public virtual AsmEvent this[ushort label]
        {
            get { return GetEventByLabel(label); }
            set { SetEventByIndex(label - Label, value); }
        }

        public virtual int Count
        {
            get { return 2 + Scripts.Count; }
        }

        public virtual IEnumerable<AsmEvent> GetOrderedEvents()
        {
            yield return Construct;
            yield return Initialize;
            foreach (AsmEvent script in Scripts.Values.OrderBy(s => s.Label))
                yield return script;
        }

        public virtual void SetEventByIndex(int index, AsmEvent script)
        {
            switch (index)
            {
                case 0:
                    Construct = script;
                    break;
                case 1:
                    Initialize = script;
                    break;
                default:
                    Scripts[script.Label] = script;
                    break;
            }
        }

        public virtual AsmEvent GetEventByLabel(ushort label)
        {
            switch (label - Label)
            {
                case 0:
                    return Construct;
                case 1:
                    return Initialize;
                default:
                    return GetScriptByLabel(label);
            }
        }

        protected AsmEvent GetScriptByLabel(ushort label)
        {
            AsmEvent result;
            if (Scripts.TryGetValue(label, out result))
                return result;

            return ((label < Label) ? PreviousModule : NextModule).GetEventByLabel(label);
        }
    }
}