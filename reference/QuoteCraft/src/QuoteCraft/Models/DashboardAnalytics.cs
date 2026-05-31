namespace QuoteCraft.Models;

public partial record DashboardAnalytics(
    decimal TotalQuotedThisMonth,
    int QuotesSentThisMonth,
    double AcceptanceRate,
    double[] QuotedDailyValues,
    double[] SentDailyValues,
    double[] AcceptanceDailyValues);
