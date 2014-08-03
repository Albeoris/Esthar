using System.Collections.Concurrent;

namespace Esthar.Core
{
    public static class ConcurrentDictionaryExm
    {
        public static TValue Remove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dic, TKey key)
        {
            TValue value;
            return dic.TryRemove(key, out value) ? value : default(TValue);
        }
    }
}