using Microsoft.UI.Xaml.Data;

namespace QuoteCraft.Helpers;

public class ShortDateConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        DateTimeOffset date;
        if (value is DateTimeOffset dto)
            date = dto;
        else if (value is string dateStr && DateTimeOffset.TryParse(dateStr, out date))
        { }
        else
            return string.Empty;

        return date.ToString("MMM dd, yyyy");
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}
