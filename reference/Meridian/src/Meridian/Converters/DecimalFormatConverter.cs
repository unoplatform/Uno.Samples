using Microsoft.UI.Xaml.Data;

namespace Meridian.Converters;

public sealed class DecimalFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var format = parameter as string ?? "N2";
        return value switch
        {
            decimal d => d.ToString(format),
            double d => d.ToString(format),
            int i => i.ToString(format),
            _ => value?.ToString() ?? ""
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}
