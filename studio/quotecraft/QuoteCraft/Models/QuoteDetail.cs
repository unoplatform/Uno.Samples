namespace QuoteCraft.Models;

public partial record QuoteDetail(QuoteEntity Quote, IImmutableList<LineItemEntity> LineItems, ClientEntity? Client = null)
{
    /// <summary>Shows the Reopen banner (PRD F17.3).</summary>
    public bool IsExpired => Quote.Status == QuoteStatus.Expired;

    /// <summary>The line items as table rows, every other one striped.</summary>
    public IImmutableList<LineItemRow> Rows => LineItems.Select((item, i) => new LineItemRow(item, i % 2 == 1)).ToImmutableList();
}

/// <summary>A line item in the detail's table; <see cref="IsAlternate"/> rows carry the stripe.</summary>
public record LineItemRow(LineItemEntity Item, bool IsAlternate);
