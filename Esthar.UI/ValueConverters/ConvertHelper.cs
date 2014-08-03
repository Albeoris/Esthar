using System;

namespace Esthar.UI
{
    public static class ConvertHelper
    {
        public static TResult Safe<TResult, TValue>(Func<TValue, object> converter, TValue value, TResult defaultValue = default(TResult))
        {
            try
            {
                return (TResult)converter(value);
            }
            catch (InvalidCastException)
            {
                throw;
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}