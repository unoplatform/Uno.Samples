using Microsoft.UI.Xaml.Data;

namespace UnoCRM.Converters;

/// <summary>
/// Maps a fraction (0..1) to a star <see cref="GridLength"/> so a progress/funnel bar's fill and
/// remainder columns can be driven from a single numeric model value — keeping the UI construct
/// (GridLength) in XAML instead of the data layer. The filled column binds the value
/// directly; the empty column passes <c>ConverterParameter="Remainder"</c> to get <c>1 - value</c>.
/// </summary>
public sealed partial class DoubleToGridLengthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var fraction = value switch
        {
            double d => d,
            float f => f,
            _ => 0d,
        };

        fraction = Math.Clamp(fraction, 0d, 1d);

        if (parameter is string s && s.Equals("Remainder", StringComparison.OrdinalIgnoreCase))
        {
            fraction = 1d - fraction;
        }

        return new GridLength(fraction, GridUnitType.Star);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}
