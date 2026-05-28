using Microsoft.UI.Xaml.Data;

namespace QuoteCraft.Helpers;

public class ChevronConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool isExpanded)
            return isExpanded ? "\uE70D" : "\uE70E"; // ChevronDown : ChevronRight
        return "\uE70E";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}
