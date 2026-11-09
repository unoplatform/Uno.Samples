namespace ClaudeCodeTracker.Presentation.MockData;

/// <summary>
/// Design-time DataContext for <see cref="SessionsPage"/> in Hot Design / Studio, built to the recipe
/// documented on <see cref="DashboardPageMockData"/>. SessionsModel's search text and model filter are
/// reactive states a design surface can't pump, so they are exposed here as plain settable values —
/// the page two-way binds them and a preview only has to RENDER a query, not run one. The session list
/// stays a list feed, because the page's FeedView can only subscribe to a feed.
/// </summary>
[ReactiveBindable]
public partial record SessionsPageMockData
{
    /// <summary>The full history, no filter applied.</summary>
    public static SessionsPageMockDataViewModel Data => new();

    /// <summary>
    /// A query that matches nothing, so the FeedView falls through to its NoneTemplate ("No sessions
    /// match your search"). The search box shows the query that produced it, so the preview reads as a
    /// search that came up empty rather than as an app with no data.
    /// </summary>
    public static SessionsPageMockDataViewModel NoResults =>
        SessionsPageMockDataViewModel.ForModel(new()
        {
            Query = "payment-gateway",
            Sessions = ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<SessionEntry>>(
                ImmutableList<SessionEntry>.Empty)),
        });

    /// <summary>
    /// One model's sessions only, with the matching chip selected — the state a tap on "Opus"
    /// produces, which a design surface cannot reach by tapping.
    /// </summary>
    public static SessionsPageMockDataViewModel OpusOnly =>
        SessionsPageMockDataViewModel.ForModel(new()
        {
            ModelFilter = "Opus",
            Sessions = ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<SessionEntry>>(
                SampleData.Sessions.Where(s => s.ModelDisplayName.Contains("Opus")).ToImmutableList())),
        });

    public string TotalCountDisplay { get; init; } =
        $"{Fmt.Count(SampleData.TotalSessions)} total sessions";

    // Settable plain strings rather than states: the page two-way binds them, and a preview only has
    // to render the selection. The vocabulary must stay materialized or the ChipGroup would drop it.
    public string Query { get; set; } = string.Empty;
    public string ModelFilter { get; set; } = SampleData.AllModels;

    public IReadOnlyList<string> FilterOptions { get; init; } = SampleData.ModelFilters;

    // An inline lambda that captures nothing, so all instances share one feed (rule 4).
    public IListFeed<SessionEntry> Sessions { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult(SampleData.Sessions.ToImmutableList()));
}

// Reaches the generated ViewModel's protected model-taking constructor, so the variants above can
// wrap a customized model. See DashboardPageMockDataViewModel for the full explanation.
public partial class SessionsPageMockDataViewModel
{
    internal static SessionsPageMockDataViewModel ForModel(SessionsPageMockData model) => new(model);
}
