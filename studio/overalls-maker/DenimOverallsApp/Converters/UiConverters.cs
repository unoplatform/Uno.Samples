using System.Globalization;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace DenimOverallsApp.Converters;

/// <summary>Converts a "#RRGGBB" / "#AARRGGBB" hex string into a <see cref="SolidColorBrush"/>.</summary>
public sealed partial class HexColorToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => new SolidColorBrush(HexColor.Parse(value as string));

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}

/// <summary>Parsing helpers for "#RRGGBB" / "#AARRGGBB" denim colour strings.</summary>
internal static class HexColor
{
    public static Color Parse(string? hex)
    {
        if (string.IsNullOrWhiteSpace(hex))
        {
            return Microsoft.UI.Colors.Transparent;
        }

        var span = hex.AsSpan().TrimStart('#');

        byte a = 0xFF, r, g, b;
        if (span.Length == 8)
        {
            a = byte.Parse(span[..2], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            r = byte.Parse(span.Slice(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            g = byte.Parse(span.Slice(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            b = byte.Parse(span.Slice(6, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        }
        else if (span.Length == 6)
        {
            r = byte.Parse(span[..2], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            g = byte.Parse(span.Slice(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            b = byte.Parse(span.Slice(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        }
        else
        {
            return Microsoft.UI.Colors.Transparent;
        }

        return Color.FromArgb(a, r, g, b);
    }

    /// <summary>Perceived brightness (0 = black, 1 = white) using the Rec. 601 luma weights.</summary>
    public static double Luminance(Color color)
        => (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255d;
}

/// <summary>
/// Picks a readable embroidery thread colour for the bound denim hex: the golden topstitch
/// thread on dark denim, switching to a dark charcoal on light washes so the text stays legible.
/// </summary>
public sealed partial class EmbroideryThreadBrushConverter : IValueConverter
{
    /// <summary>Washes brighter than this read better with dark thread than with the gold.</summary>
    public double LightThreshold { get; set; } = 0.6d;

    private static readonly SolidColorBrush GoldThread = new(Color.FromArgb(0xFF, 0xE8, 0xB9, 0x5C));
    private static readonly SolidColorBrush DarkThread = new(Color.FromArgb(0xFF, 0x2A, 0x2A, 0x2A));

    public object Convert(object value, Type targetType, object parameter, string language)
        => HexColor.Luminance(HexColor.Parse(value as string)) > LightThreshold ? DarkThread : GoldThread;

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}

/// <summary>Converts a boolean into <see cref="Visibility"/> (true =&gt; Visible). Invert with parameter "invert".</summary>
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

/// <summary>Converts the selected flag into a border thickness used to highlight the chosen option card.</summary>
public sealed partial class SelectionToThicknessConverter : IValueConverter
{
    public double SelectedThickness { get; set; } = 2d;

    public object Convert(object value, Type targetType, object parameter, string language)
        => new Thickness(value is true ? SelectedThickness : 0d);

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}
