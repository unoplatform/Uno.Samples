using System.Globalization;

namespace QuoteCraft.Helpers;

/// <summary>The checks Settings runs before it saves the quote defaults.</summary>
public static class SettingsValidation
{
    /// <returns>A message for the first value that is wrong, or null when all are valid.</returns>
    public static string? Validate(string? taxRate, string? markup, string? validDays)
    {
        if (!Money.TryParseRate(taxRate, out var tax) || tax > 100)
        {
            return "Tax rate must be a number between 0 and 100.";
        }

        if (!string.IsNullOrWhiteSpace(markup)
            && !Money.TryParseRate(markup, out _))
        {
            return "Markup must be a number of 0 or more.";
        }

        if (!int.TryParse(validDays, NumberStyles.Integer, CultureInfo.CurrentCulture, out var days) || days < 1)
        {
            return "Quotes must stay valid for at least 1 day.";
        }

        return null;
    }
}
