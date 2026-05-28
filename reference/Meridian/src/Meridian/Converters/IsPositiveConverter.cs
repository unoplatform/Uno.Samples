using Microsoft.UI.Xaml.Data;

namespace Meridian.Converters;

public sealed class IsPositiveConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value switch
        {
            decimal d => d >= 0,
            double d => d >= 0,
            int i => i >= 0,
            bool b => b,
            _ => true
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}
