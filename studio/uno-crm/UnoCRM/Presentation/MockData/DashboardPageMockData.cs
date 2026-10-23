namespace UnoCRM.Presentation.MockData;

/// <summary>
/// Design-time DataContext for <see cref="DashboardPage"/> in Hot Design / Studio, built to the
/// recipe documented on <see cref="ContactsPageMockData"/>. The four KPI read-outs and the funnel
/// are all paths off one materialized <see cref="DashboardData"/>; only the activity list is a feed,
/// because a FeedView renders it. The generated DashboardModel VM overrides this at runtime.
/// </summary>
/// <remarks>
/// A populated list does not always appear on a design surface, and the evidence says it is not the
/// mock's shape that decides it. The Contacts list renders; this page's activity list does not, from
/// mocks that are identical in shape — same feed type, same FeedView states, byte-identical item
/// template. The one structural difference is that ContactsPage also binds its list to a page-level
/// property whose code-behind ENUMERATES it while loading, which materializes the collection the
/// FeedView is showing. Nothing touches the activity collection but its own ItemsControl. What keeps
/// this panel legible instead is that its FeedView templates the UNRESOLVED state as well as the
/// empty one, so a panel with nothing yet to show has the shape of what is coming rather than no
/// height at all. Design the states and the preview cannot come out blank, whatever the host does.
/// </remarks>
[ReactiveBindable]
public partial record DashboardPageMockData
{
    // Declared first: the statics below construct instances that read these.
    private static readonly DashboardData Seed = CrmData.Dashboard;

    // A tenant a fortnight into a trial: two-figure volumes, a pipeline that thins out to nothing past
    // the middle, and revenue and conversion both slipping. Every number here differs from the busy
    // tenant's, because two previews that share their figures only prove the page renders twice.
    private static readonly DashboardData QuietSeed = new()
    {
        TotalLeadsText = "128",
        TotalLeadsDelta = "+0.4%",
        TotalLeadsTrend = KpiTrend.Up,
        ActiveDealsText = "3",
        ActiveDealsDelta = "0.0%",
        ActiveDealsTrend = KpiTrend.Flat,
        RevenueText = "$18K",
        RevenueDelta = "-6.2%",
        RevenueTrend = KpiTrend.Down,
        ConversionRateText = "2.4%",
        ConversionRateDelta = "-1.8%",
        ConversionRateTrend = KpiTrend.Down,
        Funnel =
        [
            new() { Count = 3, FillFraction = 1d },
            new() { Count = 2, FillFraction = 0.66d },
            new() { Count = 1, FillFraction = 0.33d },
            new() { Count = 0, FillFraction = 0d },
            new() { Count = 0, FillFraction = 0d },
        ],
    };

    /// <summary>A busy tenant: four-figure volumes, every KPI rising, a full funnel and a live feed.</summary>
    public static DashboardPageMockDataViewModel Data => new();

    /// <summary>
    /// A quiet account: small volumes, two KPIs falling and one flat, a funnel that empties out past
    /// Proposal, and the activity FeedView on its empty state. Laid beside the busy tenant it exercises
    /// all three delta directions and both ends of the funnel's range.
    /// </summary>
    public static DashboardPageMockDataViewModel QuietAccount =>
        DashboardPageMockDataViewModel.ForModel(new()
        {
            Overview = QuietSeed,
            TotalLeadsText = QuietSeed.TotalLeadsText,
            ActiveDealsText = QuietSeed.ActiveDealsText,
            RevenueText = QuietSeed.RevenueText,
            ConversionRateText = QuietSeed.ConversionRateText,
            Activities = ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<ActivityItem>>(ImmutableList<ActivityItem>.Empty)),
        });

    // The Overview payload: the page's funnel indexers and its four delta read-outs are all paths off
    // it, so it must be a materialized DashboardData exactly as at runtime.
    public DashboardData Overview { get; init; } = Seed;

    public string TotalLeadsText { get; init; } = Seed.TotalLeadsText;
    public string ActiveDealsText { get; init; } = Seed.ActiveDealsText;
    public string RevenueText { get; init; } = Seed.RevenueText;
    public string ConversionRateText { get; init; } = Seed.ConversionRateText;

    // An inline lambda that captures nothing, so all instances share one feed — rule 4 of the recipe.
    public IListFeed<ActivityItem> Activities { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<ActivityItem>>(CrmData.Dashboard.Activities.ToImmutableList()));
}

// Reaches the generated ViewModel's protected model-taking constructor, so QuietAccount can wrap a
// customized model. See ContactsPageMockDataViewModel for the full explanation.
public partial class DashboardPageMockDataViewModel
{
    internal static DashboardPageMockDataViewModel ForModel(DashboardPageMockData model) => new(model);
}
