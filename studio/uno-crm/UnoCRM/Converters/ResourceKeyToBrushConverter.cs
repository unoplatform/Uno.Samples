using Microsoft.UI.Xaml.Data;

namespace UnoCRM.Converters;

/// <summary>
/// Maps a theme-resource key (e.g. <c>"DashboardRedBrush"</c>) carried on a data item to
/// the actual <see cref="Brush"/> from the app resources, so per-item accent colors can come
/// from the dataset instead of being hardcoded in XAML. Resolved against
/// <see cref="Application.Current"/>'s resources, which returns the active theme variant.
/// </summary>
public sealed partial class ResourceKeyToBrushConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string key
            && Application.Current.Resources.TryGetValue(key, out var resource)
            && resource is Brush brush)
        {
            return brush;
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}
