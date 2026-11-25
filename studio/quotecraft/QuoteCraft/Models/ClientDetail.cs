namespace QuoteCraft.Models;

public partial record ClientDetail(ClientDisplayItem Client, IImmutableList<QuoteEntity> Quotes);
