using System.Collections.Generic;
using System.Linq;

namespace Esthar.Data.Transform
{
    public sealed class AsmCollection
    {
        private readonly Dictionary<ushort, AsmModule> _dic;

        public AsmCollection()
        {
            _dic = new Dictionary<ushort, AsmModule>();
        }

        public AsmCollection(int capacity)
        {
            _dic = new Dictionary<ushort, AsmModule>(capacity);
        }

        public AsmModule this[ushort label]
        {
            get { return _dic[label]; }
            set { _dic[label] = value; }
        }

        public int Count
        {
            get { return _dic.Count; }
        }

        public void Add(ushort label, AsmModule module)
        {
            _dic.Add(label, module);
            module.ParentCollection = this;
        }

        public IEnumerable<AsmModule> GetOrderedModules()
        {
            return _dic.Values.OrderBy(m => m.ExecutionOrder);
        }

        public AsmModule GetModuleByIndex(int index)
        {
            return _dic.Values.FirstOrDefault(m => m.Index == index);
        }
    }
}