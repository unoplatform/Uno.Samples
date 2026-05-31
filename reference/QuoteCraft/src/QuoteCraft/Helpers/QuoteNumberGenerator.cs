using QuoteCraft.Data;

namespace QuoteCraft.Helpers;

public class QuoteNumberGenerator
{
    private readonly IQuoteRepository _quoteRepo;
    private readonly IBusinessProfileRepository _profileRepo;

    public QuoteNumberGenerator(IQuoteRepository quoteRepo, IBusinessProfileRepository profileRepo)
    {
        _quoteRepo = quoteRepo;
        _profileRepo = profileRepo;
    }

    public async Task<string> GenerateAsync()
    {
        var profile = await _profileRepo.GetAsync();
        var count = await _quoteRepo.GetQuoteCountAsync();
        var year = DateTime.UtcNow.Year;
        var number = count + 1;
        return $"{profile.QuoteNumberPrefix}{year}-{number:D4}";
    }
}
