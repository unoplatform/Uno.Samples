namespace QuoteCraft.Models;

public partial record QuotePreviewData(
    QuoteEntity Quote,
    IImmutableList<LineItemEntity> LineItems,
    BusinessProfileEntity Profile,
    ClientEntity? Client);
