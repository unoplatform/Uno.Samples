using NSubstitute;
using QuoteCraft.Data;
using QuoteCraft.Helpers;
using QuoteCraft.Models;
using Xunit;

namespace QuoteCraft.Tests.Helpers;

public class QuoteNumberGeneratorTests
{
    private readonly IQuoteRepository _quoteRepo = Substitute.For<IQuoteRepository>();
    private readonly IBusinessProfileRepository _profileRepo = Substitute.For<IBusinessProfileRepository>();

    [Fact]
    public async Task Generate_ReturnsCorrectFormat()
    {
        _profileRepo.GetAsync().Returns(new BusinessProfileEntity { QuoteNumberPrefix = "QC-" });
        _quoteRepo.GetQuoteCountAsync().Returns(0);

        var gen = new QuoteNumberGenerator(_quoteRepo, _profileRepo);
        var result = await gen.GenerateAsync();

        Assert.StartsWith("QC-", result);
        Assert.Contains(DateTime.UtcNow.Year.ToString(), result);
        Assert.EndsWith("-0001", result);
    }

    [Fact]
    public async Task Generate_WithCustomPrefix_UsesPrefix()
    {
        _profileRepo.GetAsync().Returns(new BusinessProfileEntity { QuoteNumberPrefix = "INV-" });
        _quoteRepo.GetQuoteCountAsync().Returns(0);

        var gen = new QuoteNumberGenerator(_quoteRepo, _profileRepo);
        var result = await gen.GenerateAsync();

        Assert.StartsWith("INV-", result);
    }

    [Fact]
    public async Task Generate_WithExistingQuotes_IncrementsNumber()
    {
        _profileRepo.GetAsync().Returns(new BusinessProfileEntity { QuoteNumberPrefix = "QC-" });
        _quoteRepo.GetQuoteCountAsync().Returns(42);

        var gen = new QuoteNumberGenerator(_quoteRepo, _profileRepo);
        var result = await gen.GenerateAsync();

        Assert.EndsWith("-0043", result);
    }
}
