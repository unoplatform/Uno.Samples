using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace UnoChat.Converters;

public class StringEqualityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return (value, parameter) switch
        {
            (null, _) => false,
            (_, null) => false,
            (string x, string y) => x.Equals(y, StringComparison.Ordinal),
            (object x, string y) => x.ToString().Equals(y, StringComparison.Ordinal),
            _ => DependencyProperty.UnsetValue
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotSupportedException();
    }
}
