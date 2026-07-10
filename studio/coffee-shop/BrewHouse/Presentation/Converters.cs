using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace BrewHouse.Presentation;

/// <summary>
/// Boolean to <see cref="Visibility"/> — a content-free value transform so view-models can expose
/// plain <c>bool</c> state and let XAML decide visibility. Pass <c>Invert</c> as the parameter to flip.
/// </summary>
public partial class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var flag = value is bool b && b;
        if (string.Equals(parameter as string, "Invert", StringComparison.OrdinalIgnoreCase))
            flag = !flag;
        return flag ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}
