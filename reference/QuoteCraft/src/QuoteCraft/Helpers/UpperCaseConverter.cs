using Microsoft.UI.Xaml.Data;

namespace QuoteCraft.Helpers;

public class UpperCaseConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => value is string s ? s.ToUpperInvariant() : value;

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}
