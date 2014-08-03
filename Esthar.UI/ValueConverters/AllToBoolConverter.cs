using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Esthar.UI
{
    public sealed class AllToBoolConverter<T> : IMultiValueConverter
    {
        private readonly T _value;

        public AllToBoolConverter(T value)
        {
            _value = value;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length == 0)
                return true;

            return values.All(value => ((T)value).Equals(_value));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}