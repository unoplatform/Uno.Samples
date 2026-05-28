using System.Globalization;
using Microsoft.UI.Xaml.Data;

namespace QuoteCraft.Helpers;

public class CurrencyFormatConverter : IValueConverter
{
    /// <summary>
    /// Set by the app on startup and when currency changes.
    /// Supported: USD, CAD, EUR, GBP, AUD, MXN.
    /// </summary>
    public static string CurrencyCode { get; set; } = "USD";

    private static readonly Dictionary<string, CultureInfo> CurrencyCultures = new()
    {
        ["USD"] = new CultureInfo("en-US"),
        ["CAD"] = new CultureInfo("en-CA"),
        ["EUR"] = new CultureInfo("fr-FR"),
        ["GBP"] = new CultureInfo("en-GB"),
        ["AUD"] = new CultureInfo("en-AU"),
        ["MXN"] = new CultureInfo("es-MX"),
    };

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var amount = value switch
        {
            double d => d,
            float f => (double)f,
            decimal m => (double)m,
            int i => (double)i,
            long l => (double)l,
            string s when double.TryParse(s, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed) => parsed,
            _ => 0.0
        };

        if (CurrencyCultures.TryGetValue(CurrencyCode, out var culture))
            return amount.ToString("C2", culture);

        // Invariant fallback for unknown currency codes
        return $"{CurrencyCode} {amount:#,##0.00}";
    }

    public static string GetCurrencySymbol(string currencyCode)
    {
        if (CurrencyCultures.TryGetValue(currencyCode, out var culture))
        {
            return culture.NumberFormat.CurrencySymbol;
        }
        return currencyCode;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}
