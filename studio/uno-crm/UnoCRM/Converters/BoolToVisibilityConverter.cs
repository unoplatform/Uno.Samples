using Microsoft.UI.Xaml.Data;

namespace UnoCRM.Converters;

/// <summary>
/// Maps a <see cref="bool"/> to <see cref="Visibility"/>: <c>true</c> → Visible, <c>false</c> →
/// Collapsed. Pass <c>ConverterParameter="Invert"</c> to flip the mapping (e.g. to show an
/// empty-state panel when a "has results" flag is false).
/// </summary>
public sealed partial class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var flag = value is bool b && b;

        if (parameter is string s && s.Equals("Invert", StringComparison.OrdinalIgnoreCase))
        {
            flag = !flag;
        }

        return flag ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}
