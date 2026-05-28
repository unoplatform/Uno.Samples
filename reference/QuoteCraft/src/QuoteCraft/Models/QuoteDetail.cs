namespace QuoteCraft.Models;

public partial record QuoteDetail(QuoteEntity Quote, IImmutableList<LineItemEntity> LineItems, ClientEntity? Client = null);
