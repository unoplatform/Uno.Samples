using Microsoft.UI.Xaml.Data;

namespace QuoteCraft.Helpers;

/// <summary>
/// True → Visible, false (or no value) → Collapsed. WinUI's {Binding} does not convert bool to Visibility (only x:Bind
/// does), so a Visibility bound to a bool needs this on the WinAppSDK head.
/// </summary>
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => value is true ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => value is Visibility.Visible;
}
