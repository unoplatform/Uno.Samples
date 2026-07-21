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

/// <summary>
/// Maps a <c>TabBarItem.IsSelected</c> bool to the themed TabBar foreground brush
/// (<c>TabBarItemForegroundSelected</c> vs <c>TabBarItemForeground</c>). Custom TabBarItem content
/// (e.g. the Cart label) can't rely on the item's <c>Foreground</c> to reflect selection — that DP
/// isn't what the Simple theme drives per state — so bind the custom label to <c>IsSelected</c>
/// through this converter to match the string-content tabs. Resolves the current theme's brush at
/// bind time (right for an app that follows the OS theme).
/// </summary>
public partial class TabSelectionForegroundConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        var key = value is bool b && b ? "TabBarItemForegroundSelected" : "TabBarItemForeground";
        return Application.Current.Resources.TryGetValue(key, out var brush) ? brush : null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}
