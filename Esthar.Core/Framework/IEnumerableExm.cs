using System;
using System.Collections.Generic;
using System.Linq;

namespace Esthar.Core
{
    public static class IEnumerableExm
    {
        public static IEnumerable<TResult> SafeSelect<TSource, TResult>(this IEnumerable<TSource> self, Func<TSource, TResult> selector)
        {
            return self == null ? new TResult[0] : self.Select(selector);
        }

        public static IEnumerable<TResult> SelectWhere<TSource, TResult>(this IEnumerable<TSource> self, Func<TSource, bool> predicate, Func<TSource, TResult> selector)
        {
            return from item in self where predicate(item) select selector(item);
        }

        public static IEnumerable<T> Order<T>(this IEnumerable<T> self)
        {
            return self.OrderBy(t => t);
        }
    }
}