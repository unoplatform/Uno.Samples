using Microsoft.UI.Xaml.Data;

namespace QuoteCraft.Helpers;

public class StatusIconConverter : IValueConverter
{
    private static readonly Dictionary<string, string> Glyphs = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Draft"] = "\uE70F",     // Edit
        ["Sent"] = "\uE724",      // Send
        ["Viewed"] = "\uE7B3",    // View
        ["Accepted"] = "\uE73E",  // CheckMark
        ["Declined"] = "\uE711",  // Cancel
    };

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var status = value?.ToString() ?? string.Empty;
        return Glyphs.GetValueOrDefault(status, "\uE1D0");
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}
