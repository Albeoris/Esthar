using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Esthar.UI
{
    public sealed class AnyToBoolConverter<T> : IMultiValueConverter
    {
        private readonly T _value;

        public AnyToBoolConverter(T value)
        {
            _value = value;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length == 0)
                return false;

            return values.Any(value => _value.Equals((T)value));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}