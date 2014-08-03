using System.Collections.Generic;

namespace Esthar.Core
{
    public static class ListExm
    {
        public static T AddAndReturn<T>(this List<T> list, T item)
        {
            list.Add(item);
            return item;
        }
    }
}