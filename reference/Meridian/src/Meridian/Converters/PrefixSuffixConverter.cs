using Microsoft.UI.Xaml.Data;

namespace Meridian.Converters;

/// <summary>
/// Formats a value with prefix and/or suffix. Parameter format: "prefix|suffix"
/// Examples: "$|" for "$247.63", "|%" for "1.40%", "$|M" for "$62.1M"
/// </summary>
public sealed class PrefixSuffixConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var parts = (parameter as string)?.Split('|') ?? ["", ""];
        var prefix = parts.Length > 0 ? parts[0] : "";
        var suffix = parts.Length > 1 ? parts[1] : "";
        return $"{prefix}{value}{suffix}";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}
