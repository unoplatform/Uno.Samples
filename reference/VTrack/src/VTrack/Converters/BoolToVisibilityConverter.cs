using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace VTrack.Converters;

/// <summary>
/// Maps <c>true</c> to <see cref="Visibility.Visible"/> and <c>false</c> to
/// <see cref="Visibility.Collapsed"/>. Used in place of Uno's implicit bool→Visibility
/// conversion so bindings behave the same across all target heads.
/// </summary>
public sealed class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => value is true ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => value is Visibility.Visible;
}
