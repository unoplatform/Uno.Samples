using Microsoft.UI.Xaml.Data;

namespace QuoteCraft.Helpers;

public class InitialsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var name = value as string;
        if (string.IsNullOrWhiteSpace(name))
            return "?";

        var parts = name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2)
            return $"{char.ToUpper(parts[0][0])}{char.ToUpper(parts[^1][0])}";

        return parts[0].Length >= 2
            ? $"{char.ToUpper(parts[0][0])}{char.ToUpper(parts[0][1])}"
            : $"{char.ToUpper(parts[0][0])}";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}
