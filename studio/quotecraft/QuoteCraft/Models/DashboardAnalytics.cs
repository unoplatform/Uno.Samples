namespace QuoteCraft.Models;

public partial record DashboardAnalytics(
    decimal TotalQuotedThisMonth,
    int QuotesSentThisMonth,
    double AcceptanceRate,
    double[] QuotedDailyValues,
    double[] SentDailyValues,
    double[] AcceptanceDailyValues)
{
    private const int SparklineDays = 7;

    /// <summary>
    /// The Quotes page's stat cards: this month's quoted total and quotes sent (anything past Draft), the acceptance
    /// rate over every answered quote (accepted of accepted and declined), and the last 7 days for the sparklines.
    /// </summary>
    public static DashboardAnalytics From(IReadOnlyCollection<QuoteEntity> all, DateTimeOffset now)
    {
        var startOfMonth = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, TimeSpan.Zero);
        var thisMonth = all.Where(q => q.CreatedAt >= startOfMonth).ToList();

        var answered = all.Count(q => q.Status is QuoteStatus.Accepted or QuoteStatus.Declined);
        var accepted = all.Count(q => q.Status == QuoteStatus.Accepted);
        var rate = answered > 0 ? Math.Round((double)accepted / answered * 100, 1) : 0;

        // Group the last 7 days' quotes by day once, then index.
        var quotedDaily = new double[SparklineDays];
        var sentDaily = new double[SparklineDays];
        var acceptanceDaily = new double[SparklineDays];
        var baseDate = now.Date.AddDays(-(SparklineDays - 1));
        var byDay = all.Where(q => q.CreatedAt >= baseDate)
            .GroupBy(q => (int)(q.CreatedAt.Date - baseDate).TotalDays)
            .Where(g => g.Key is >= 0 and < SparklineDays)
            .ToDictionary(g => g.Key, g => g.ToList());

        for (var i = 0; i < SparklineDays; i++)
        {
            if (byDay.TryGetValue(i, out var day))
            {
                quotedDaily[i] = (double)day.Sum(q => q.Total);
                sentDaily[i] = day.Count(q => q.Status != QuoteStatus.Draft);
            }
            acceptanceDaily[i] = rate;
        }

        return new DashboardAnalytics(
            thisMonth.Sum(q => q.Total),
            thisMonth.Count(q => q.Status != QuoteStatus.Draft),
            rate,
            quotedDaily,
            sentDaily,
            acceptanceDaily);
    }
}
