using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace ClaudeCodeTracker.Converters;

/// <summary>
/// Converts a boolean into <see cref="Visibility"/> (true =&gt; Visible). Pass the parameter
/// "invert" to flip the mapping. Lets a model expose a plain data flag while XAML decides how
/// to render it, instead of the model returning a <see cref="Visibility"/> directly.
/// </summary>
public sealed partial class BooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var flag = value is true;
        if (string.Equals(parameter as string, "invert", StringComparison.OrdinalIgnoreCase))
        {
            flag = !flag;
        }

        return flag ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => value is Visibility.Visible;
}

/// <summary>
/// Wraps a <see cref="Color"/> in a <see cref="SolidColorBrush"/> at bind time, so a model can
/// expose colours as plain data (<see cref="Color"/>) and leave the UI-layer brush to XAML.
/// </summary>
public sealed partial class ColorToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => new SolidColorBrush(value is Color color ? color : Microsoft.UI.Colors.Transparent);

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => value is SolidColorBrush brush ? brush.Color : Microsoft.UI.Colors.Transparent;
}
