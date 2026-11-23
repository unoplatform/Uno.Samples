using System.Globalization;

namespace QuoteCraft.Models;

/// <summary>A line of the Settings preview quote.</summary>
public record SettingsPreviewLine(string Description, int Quantity, string UnitPrice, string LineTotal);

/// <summary>
/// The sample quote Settings shows beside the form, worked out from the defaults as typed: what a new quote with these
/// settings looks like. Unit prices carry the markup (catalog items are added at their price plus the markup), tax is
/// the default rate on the subtotal (<see cref="Money"/>), and the dates run from today.
/// </summary>
public partial record SettingsPreview(
    string QuoteNumber,
    string Date,
    string ValidUntil,
    IImmutableList<SettingsPreviewLine> Lines,
    string Subtotal,
    string TaxLabel,
    string Tax,
    string Total,
    string Currency)
{
    /// <summary>Catalog prices before markup: (description, unit price, quantity).</summary>
    private static readonly (string Description, decimal Price, int Quantity)[] SampleItems =
    [
        ("Copper pipe 1/2\"", 12.50m, 10),
        ("Labor - Installation", 75.00m, 4),
        ("Fittings & connectors", 4.50m, 8),
    ];

    public static SettingsPreview From(string? prefix, string? taxRate, string? markup, string? validDays, string? currencyCode, DateTimeOffset today)
    {
        var currency = string.IsNullOrWhiteSpace(currencyCode) ? "USD" : currencyCode.Trim().ToUpperInvariant();
        var markupPercent = Money.TryParseRate(markup, out var m) ? m : 0m;
        var lines = SampleItems
            .Select(item =>
            {
                var unit = Money.WithMarkup(item.Price, markupPercent);
                return (item.Description, item.Quantity, Unit: unit, Total: Money.LineTotal(unit, item.Quantity));
            })
            .ToList();

        var subtotal = lines.Sum(l => l.Total);
        var rateReads = Money.TryParseRate(taxRate, out var rate) && rate <= 100;
        var tax = rateReads ? Money.Tax(subtotal, rate) : 0m;
        var days = int.TryParse(validDays, NumberStyles.Integer, CultureInfo.CurrentCulture, out var d) && d > 0 ? d : (int?)null;

        return new SettingsPreview(
            QuoteNumberGenerator.Format(string.IsNullOrWhiteSpace(prefix) ? "QC-" : prefix.Trim(), today.Year, 1),
            today.ToString("MMM dd, yyyy", CultureInfo.CurrentCulture),
            days is { } valid ? today.AddDays(valid).ToString("MMM dd, yyyy", CultureInfo.CurrentCulture) : "—",
            lines.Select(l => new SettingsPreviewLine(l.Description, l.Quantity, Money.Format(l.Unit, currency), Money.Format(l.Total, currency)))
                .ToImmutableList(),
            Money.Format(subtotal, currency),
            rateReads ? $"Tax ({Money.FormatRate(rate)} %)" : "Tax (enter a rate from 0 to 100)",
            Money.Format(tax, currency),
            Money.Format(subtotal + tax, currency),
            currency);
    }
}
