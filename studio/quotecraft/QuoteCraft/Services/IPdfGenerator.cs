namespace QuoteCraft.Services;

public interface IPdfGenerator
{
    /// <summary>The quote as a PDF document (US Letter), with the business profile's details.</summary>
    Task<byte[]> GenerateQuotePdfAsync(QuoteEntity quote);
}
