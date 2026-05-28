namespace QuoteCraft.Services;

public interface IPdfGenerator
{
    Task<string> GenerateQuotePdfAsync(QuoteEntity quote);
}
