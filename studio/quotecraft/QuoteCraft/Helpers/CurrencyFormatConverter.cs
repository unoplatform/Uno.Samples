using Microsoft.UI.Xaml.Data;

namespace QuoteCraft.Helpers;

/// <summary>Shows an amount in the business's currency (<see cref="Money.Format"/>), the way the PDF shows it.</summary>
public class CurrencyFormatConverter : IValueConverter
{
    /// <summary>
    /// The app's <see cref="CurrencyService"/>. XAML creates this converter (an App.xaml resource), so it cannot take
    /// the service from DI: App hands it the instance it registers. Until then amounts read in USD.
    /// </summary>
    public CurrencyService? Currency { get; set; }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var amount = value switch
        {
            decimal m => m,
            int i => i,
            long l => l,
            double d when double.IsFinite(d) => (decimal)d,
            _ => 0m,
        };
        return Money.Format(amount, Currency?.Code ?? "USD");
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}
