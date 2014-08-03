using System;
using System.Collections;
using System.Collections.Generic;

namespace Esthar.Data.Transform
{
    public sealed class LocalizableStrings : IList<LocalizableString>
    {
        private readonly List<LocalizableString> _strings;

        public LocalizableStrings()
        {
            _strings = new List<LocalizableString>();
        }

        public LocalizableStrings(int capacity)
        {
            _strings = new List<LocalizableString>(capacity);
        }

        public LocalizableStrings(IEnumerable<LocalizableString> collection)
        {
            _strings = new List<LocalizableString>(collection);
        }

        public LocalizableString this[int index]
        {
            get { return _strings[index]; }
            set
            {
                value.Index = index;
                if (value.Order < 0) value.Order = index;
                _strings[index] = value;
            }
        }

        public int Count
        {
            get { return _strings.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Add(LocalizableString item)
        {
            item.Index = _strings.Count;
            if (item.Order < 0) item.Order = item.Index;
            _strings.Add(item);
        }

        public void Insert(int index, LocalizableString item)
        {
            throw new NotSupportedException();
        }

        public bool Remove(LocalizableString item)
        {
            int index = IndexOf(item);
            if (index < 0)
                return false;

            RemoveAt(index);
            return true;
        }

        public void RemoveAt(int index)
        {
            _strings.RemoveAt(index);
        }

        public void Clear()
        {
            _strings.Clear();
        }

        public int IndexOf(LocalizableString item)
        {
            if (item == null || item.Index < 0 || item.Index >= _strings.Count)
                return _strings.IndexOf(item);

            if (item.Equals(_strings[item.Index]))
                return item.Index;

            return _strings.IndexOf(item);
        }

        public bool Contains(LocalizableString item)
        {
            return IndexOf(item) >= 0;
        }

        public void CopyTo(LocalizableString[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<LocalizableString> GetEnumerator()
        {
            return _strings.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}