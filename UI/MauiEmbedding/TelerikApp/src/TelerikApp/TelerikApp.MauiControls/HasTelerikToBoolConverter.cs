using System.Globalization;

namespace TelerikApp.MauiControls;

public class HasTelerikToBoolConverter : IValueConverter
{

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
		return 
#if HAS_TELERIK
			true;
#else
			false;	
#endif
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
