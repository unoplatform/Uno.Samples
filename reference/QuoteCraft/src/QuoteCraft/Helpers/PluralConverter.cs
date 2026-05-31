using Microsoft.UI.Xaml.Data;

namespace QuoteCraft.Helpers;

public class PluralConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var word = parameter as string ?? "";
        var count = System.Convert.ToInt32(value);
        return count == 1 ? $" {word}" : $" {word}s";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}
