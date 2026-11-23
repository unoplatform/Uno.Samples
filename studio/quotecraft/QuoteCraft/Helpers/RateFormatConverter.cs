using Microsoft.UI.Xaml.Data;

namespace QuoteCraft.Helpers;

/// <summary>Shows a tax or markup rate in full ("8.25", "9.975"), as <see cref="Money.FormatRate"/> does.</summary>
public class RateFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => value is decimal rate ? Money.FormatRate(rate) : string.Empty;

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}
