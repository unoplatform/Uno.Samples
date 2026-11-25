using Microsoft.UI.Xaml.Data;

namespace QuoteCraft.Helpers;

/// <summary>
/// Visible when the bound number is zero (an empty-list hint), otherwise Collapsed; <c>Invert</c> shows it for any
/// other number (a count badge).
/// </summary>
public class ZeroToVisibleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var isZero = value is int count && count == 0;
        var invert = parameter is string p && p.Equals("Invert", StringComparison.OrdinalIgnoreCase);
        return isZero != invert ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}
