namespace QuoteCraft.Models;

/// <summary>The editor's totals (PRD F1.5): what the PDF will show, by the rules in Helpers/Money.</summary>
/// <param name="TaxLabel">"Tax (8.25 %)", or a prompt while the rate does not read.</param>
public partial record QuoteTotals(decimal Subtotal, string TaxLabel, decimal Tax, decimal Total);
