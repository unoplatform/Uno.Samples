using Microsoft.UI.Xaml.Data;

namespace QuoteCraft.Helpers;

/// <summary>
/// Converts an integer step number to Visibility.
/// ConverterParameter is the target step number (as a string).
/// Returns Visible when value == parameter, Collapsed otherwise.
/// </summary>
public class StepVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is int step && parameter is string paramStr && int.TryParse(paramStr, out var target))
            return step == target ? Visibility.Visible : Visibility.Collapsed;

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}
