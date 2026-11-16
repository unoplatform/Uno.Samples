using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;

namespace Voyago.Presentation;

/// <summary>bool → Visibility. Pass ConverterParameter="Invert" for the complement.</summary>
public sealed partial class BoolToVisibilityConverter : IValueConverter
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
/// Resolves a vector icon <see cref="Microsoft.UI.Xaml.Media.Geometry"/>. Use
/// ConverterParameter to name an icon key directly (static icons: <c>ConverterParameter=home</c>),
/// or bind a domain label as the value (icon lists: the label maps to a key via <see cref="Icons.KeyFor"/>).
/// A fresh geometry is produced each call, so no instance is shared across the visual tree.
/// </summary>
public sealed partial class IconConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        var key = parameter is string p ? p : (value is string s ? Icons.KeyFor(s) : null);
        return Icons.Get(key);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}

/// <summary>
/// Picks a trip-status pill template by value so each status carries its own semantic colour
/// (Confirmed = primary, Pending = amber, Completed = neutral). Colour lives in XAML
/// (<c>{ThemeResource}</c> templates) rather than a resource-reading converter, which keeps it
/// theme-reactive and correct under Studio Live / Hot Design.
/// </summary>
public sealed partial class TripStatusTemplateSelector : DataTemplateSelector
{
    public DataTemplate? Confirmed { get; set; }
    public DataTemplate? Pending { get; set; }
    public DataTemplate? Completed { get; set; }

    protected override DataTemplate? SelectTemplateCore(object item) => (item as string) switch
    {
        "Confirmed" => Confirmed,
        "Pending" => Pending,
        "Completed" => Completed,
        _ => Confirmed,
    };

    protected override DataTemplate? SelectTemplateCore(object item, DependencyObject container)
        => SelectTemplateCore(item);
}
