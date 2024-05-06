using System.Globalization;

namespace ArcGisApp;

public class UnoImageConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
#if ANDROID
        return (value + "").Replace('/','_').Replace('\\','_');
#else
        return value;
#endif
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
