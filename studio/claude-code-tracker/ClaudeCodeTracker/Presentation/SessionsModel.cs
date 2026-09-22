namespace ClaudeCodeTracker.Presentation;

// The session history with a live text search and a model chip filter. Both inputs are two-way
// states; the list is a list-feed that asks ITrackerService for the matching set whenever either
// changes — the search runs on the SERVICE, not on a client-side copy of the history, which is what
// gives the page real loading, empty and failure states to render.
//
// This replaces filtering that used to live in the page's code-behind, where it set ItemsSource
// imperatively and toggled an empty-state panel's Visibility by hand.
public partial record SessionsModel(ITrackerService Tracker)
{
    public string TotalCountDisplay => $"{Fmt.Count(SampleData.TotalSessions)} total sessions";

    /// <summary>Two-way bound to the search box.</summary>
    public IState<string> Query => State.Value(this, () => string.Empty);

    /// <summary>The active model filter; the ChipGroup's selection writes to it.</summary>
    public IState<string> ModelFilter => State.Value(this, () => SampleData.AllModels);

    /// <summary>
    /// Model filter labels ("All", "Opus", …) shown as a ChipGroup. Deliberately a plain synchronous
    /// list, not a feed: this is a fixed vocabulary — chrome the UI needs in order to ASK for data,
    /// not data itself. A Selector refuses a selection that is absent from its items, and with an
    /// async source the items arrive after the binding pushes the value, so the chip bar would render
    /// with nothing selected and write the empty selection back over <see cref="ModelFilter"/>.
    /// </summary>
    public IReadOnlyList<string> FilterOptions => SampleData.ModelFilters;

    /// <summary>
    /// The matching sessions. Both criteria are combined and handed to the service, so the page can
    /// render this through a FeedView: an empty result is the NoneTemplate ("No sessions match…") and
    /// a failed request is the ErrorTemplate.
    /// </summary>
    public IListFeed<SessionEntry> Sessions =>
        Feed.Combine(ModelFilter, Query)
            .SelectAsync(async (criteria, ct) =>
                await Tracker.SearchSessionsAsync(criteria.Item1, criteria.Item2, ct))
            .AsListFeed();
}
