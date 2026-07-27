using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace MovieStreamApp.Converters;

/// <summary>
/// bool -> Visibility. ConverterParameter="invert" flips it (true collapses). Used for the search
/// empty-state and other data-driven visibility, so the view decides how a bool renders (lesson 28).
/// </summary>
public sealed partial class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var flag = value is bool b && b;
        if (parameter as string == "invert")
        {
            flag = !flag;
        }
        return flag ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotSupportedException();
}
