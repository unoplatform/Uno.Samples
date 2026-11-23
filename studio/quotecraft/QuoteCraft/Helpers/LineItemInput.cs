using System.Globalization;

namespace QuoteCraft.Helpers;

/// <summary>
/// Reads the line-item form (PRD US-2): a description, a unit price in whole cents (see <see cref="Money"/>) and a
/// whole quantity of 1 or more. Nothing is guessed: a value that does not read cleanly gets a message instead.
/// </summary>
public static class LineItemInput
{
    /// <returns>What is wrong, worded for the form; null when the three values are read.</returns>
    public static string? TryRead(string? description, string? unitPrice, string? quantity,
        out string readDescription, out decimal readUnitPrice, out int readQuantity, CultureInfo? culture = null)
    {
        culture ??= CultureInfo.CurrentCulture;
        readDescription = description?.Trim() ?? string.Empty;
        readUnitPrice = 0;
        readQuantity = 0;

        if (readDescription.Length == 0)
        {
            return "Enter a description.";
        }

        if (!Money.TryParseAmount(unitPrice, out readUnitPrice, culture))
        {
            return $"Enter the unit price as an amount, for example {Money.FormatAmountInput(150m, culture)}.";
        }

        if (!int.TryParse(quantity?.Trim(), NumberStyles.None, culture, out readQuantity) || readQuantity < 1)
        {
            return "Enter a whole quantity of 1 or more.";
        }

        return null;
    }
}
