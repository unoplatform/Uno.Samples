using System.Text.RegularExpressions;

namespace QuoteCraft.Helpers;

/// <summary>
/// Quote numbers: the profile's prefix, the year and a sequence, e.g. QC-2026-0001 (PRD F1.9). The sequence comes from
/// a per-year counter in the database, so a number is never handed out twice, even after a delete. A number taken and
/// not saved leaves a gap; gaps are fine, reuse is not (PDF exports are named after the number).
/// </summary>
public partial class QuoteNumberGenerator
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
        return await _quoteRepo.NextQuoteNumberAsync(profile.QuoteNumberPrefix, DateTime.UtcNow.Year);
    }

    public static string Format(string prefix, int year, int sequence) => $"{prefix}{year}-{sequence:D4}";

    /// <summary>Splits "QC-2026-0012" into "QC-", 2026 and 12; false for a number in another shape.</summary>
    public static bool TryParse(string number, out string prefix, out int year, out int sequence)
    {
        var match = NumberPattern().Match(number);
        prefix = match.Success ? match.Groups["prefix"].Value : string.Empty;
        year = match.Success ? int.Parse(match.Groups["year"].Value, System.Globalization.CultureInfo.InvariantCulture) : 0;
        sequence = match.Success && int.TryParse(match.Groups["sequence"].Value, System.Globalization.CultureInfo.InvariantCulture, out var s) ? s : 0;
        return match.Success && sequence > 0;
    }

    [GeneratedRegex(@"^(?<prefix>.*?)(?<year>\d{4})-(?<sequence>\d{1,9})$")]
    private static partial Regex NumberPattern();
}
