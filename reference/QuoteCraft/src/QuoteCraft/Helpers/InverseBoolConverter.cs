using Microsoft.UI.Xaml.Data;

namespace QuoteCraft.Helpers;

public class InverseBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool b)
        {
            if (targetType == typeof(Visibility))
                return b ? Visibility.Collapsed : Visibility.Visible;
            return !b;
        }
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}
