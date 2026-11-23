using System.Globalization;

namespace QuoteCraft.Helpers;

/// <summary>
/// QuoteCraft's money rules, in one place:
/// <list type="bullet">
/// <item>Amounts are <see cref="decimal"/>, never <see cref="double"/>. Unit prices are whole cents.</item>
/// <item>A line total is unit price × quantity. Tax is subtotal × rate, rounded once, to the cent. The total is
/// subtotal + tax.</item>
/// <item>Rounding is half away from zero (0.825 → 0.83), as on an invoice; .NET's default rounds half to even.</item>
/// <item>Rates (tax, markup) are kept exactly as entered (8.25 stays 8.25, 9.975 stays 9.975) and shown in full.</item>
/// <item>What the user types is read in the user's culture (8,25 in French or German); amounts are shown in the
/// convention of the business's currency (<see cref="Format"/>).</item>
/// </list>
/// </summary>
public static class Money
{
    private static readonly Dictionary<string, CultureInfo> CurrencyCultures = new()
    {
        ["USD"] = CultureInfo.GetCultureInfo("en-US"),
        ["CAD"] = CultureInfo.GetCultureInfo("en-CA"),
        ["EUR"] = CultureInfo.GetCultureInfo("fr-FR"),
        ["GBP"] = CultureInfo.GetCultureInfo("en-GB"),
        ["AUD"] = CultureInfo.GetCultureInfo("en-AU"),
        ["MXN"] = CultureInfo.GetCultureInfo("es-MX"),
    };

    public static decimal RoundToCents(decimal amount) => Math.Round(amount, 2, MidpointRounding.AwayFromZero);

    public static decimal LineTotal(decimal unitPrice, int quantity) => RoundToCents(unitPrice * quantity);

    /// <param name="ratePercent">8.25 for 8.25 %.</param>
    public static decimal Tax(decimal subtotal, decimal ratePercent) => RoundToCents(subtotal * ratePercent / 100m);

    /// <summary>A catalog price with the business's markup, to the cent.</summary>
    public static decimal WithMarkup(decimal price, decimal markupPercent) => RoundToCents(price * (1 + markupPercent / 100m));

    /// <summary>"$1,250.00" for USD, "1 250,00 €" for EUR; an unknown code reads "XYZ 1,250.00".</summary>
    public static string Format(decimal amount, string currencyCode) =>
        CurrencyCultures.TryGetValue(currencyCode, out var culture)
            ? amount.ToString("C2", culture)
            : $"{currencyCode} {amount.ToString("#,##0.00", CultureInfo.InvariantCulture)}";

    public static string Symbol(string currencyCode) =>
        CurrencyCultures.TryGetValue(currencyCode, out var culture) ? culture.NumberFormat.CurrencySymbol : currencyCode;

    /// <summary>A rate as the user typed it: "8.25", "9.975", "13" (or "8,25" in a decimal-comma culture).</summary>
    public static string FormatRate(decimal rate, CultureInfo? culture = null) =>
        rate.ToString("0.##########", culture ?? CultureInfo.CurrentCulture);

    /// <summary>An amount for a text box: "150.00" (or "150,00"), which <see cref="TryParseAmount"/> reads back.</summary>
    public static string FormatAmountInput(decimal amount, CultureInfo? culture = null) =>
        amount.ToString("0.00", culture ?? CultureInfo.CurrentCulture);

    /// <summary>
    /// Reads a percentage such as "8.25" or "8.25 %". No thousands separators: in English "1,5" is not read as 15.
    /// </summary>
    public static bool TryParseRate(string? text, out decimal rate, CultureInfo? culture = null)
    {
        var trimmed = text?.Trim().TrimEnd('%').Trim();
        return decimal.TryParse(trimmed, NumberStyles.AllowDecimalPoint, culture ?? CultureInfo.CurrentCulture, out rate);
    }

    /// <summary>
    /// Reads a price of 0 or more in whole cents: "150", "$150", "1,250.00" (or "1.250,00"). A currency symbol is
    /// ignored. Thousands separators must group by three, so "1,5" in English is refused rather than read as 15.
    /// </summary>
    public static bool TryParseAmount(string? text, out decimal amount, CultureInfo? culture = null)
    {
        amount = 0;
        culture ??= CultureInfo.CurrentCulture;
        var digits = new string((text ?? string.Empty)
            .Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.CurrencySymbol)
            .ToArray()).Trim();
        if (digits.Length == 0 || !HasValidGrouping(digits, culture.NumberFormat))
        {
            return false;
        }

        if (!decimal.TryParse(digits, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, culture, out var value)
            || value != Math.Round(value, 2))
        {
            return false;
        }

        amount = value;
        return true;
    }

    private static bool HasValidGrouping(string text, NumberFormatInfo format)
    {
        var separator = format.NumberGroupSeparator;
        if (string.IsNullOrEmpty(separator) || !text.Contains(separator, StringComparison.Ordinal))
        {
            return true;
        }

        var decimalAt = text.IndexOf(format.NumberDecimalSeparator, StringComparison.Ordinal);
        var whole = decimalAt >= 0 ? text[..decimalAt] : text;
        var groups = whole.Split(separator);
        return groups[0].Length is >= 1 and <= 3 && groups.Skip(1).All(g => g.Length == 3);
    }
}
