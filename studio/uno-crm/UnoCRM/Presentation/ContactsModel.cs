using UnoCRM.Presentation.Services;

namespace UnoCRM.Presentation;

/// <summary>
/// Backs <see cref="ContactsPage"/>. The three filters (search text, region, segment) are two-way
/// <see cref="IState{T}"/> — the desktop and mobile controls bind the SAME state, so they stay in
/// lockstep for free (no manual mirror / sync flag).
///
/// The filtered list is a list feed that asks <see cref="ICrmService"/> for the matching set whenever
/// any filter changes: the query runs on the SERVICE, not over a client-side copy of the dataset.
/// That is what gives the page's two FeedViews all four states — before this the filter was
/// synchronous, so Progress and Error could never occur and the controls were two-state.
///
/// The page code-behind observes the list and re-renders the Mapsui map.
/// </summary>
public partial record ContactsModel(ICrmService Crm)
{
    public const string AllRegions = "All Regions";
    public const string AllSegments = "All Segments";

    // Two-way mutable user input. One state each, bound by both layouts' controls.
    public IState<string> SearchText => State.Value(this, () => string.Empty);
    public IState<string> RegionFilter => State.Value(this, () => AllRegions);
    public IState<string> SegmentFilter => State.Value(this, () => AllSegments);

    // The filter VOCABULARY, derived from the dataset with an "All …" entry first. Deliberately
    // synchronous, and the one thing on this page that is NOT a feed.
    //
    // Both ComboBoxes bind SelectedItem TwoWay to the filter states above, whose defaults are the
    // "All …" sentinels. A ComboBox cannot hold a SelectedItem that is not in its ItemsSource: if the
    // items arrive later — as they do from an async feed — the selection is cleared and both filters
    // render blank. (Verified on the simulator: making these list feeds emptied both dropdowns.)
    // These are the affordance for filtering, not content that has a loading or failure state worth
    // rendering, so they are materialized on the first frame instead.
    public IReadOnlyList<string> Regions { get; } =
        new[] { AllRegions }
            .Concat(CrmData.Contacts.Select(c => c.Region).Distinct(StringComparer.OrdinalIgnoreCase))
            .ToArray();

    public IReadOnlyList<string> Segments { get; } =
        new[] { AllSegments }
            .Concat(CrmData.Contacts.Select(c => c.Segment).Distinct(StringComparer.OrdinalIgnoreCase))
            .ToArray();

    // One filtered feed that the list, the header metrics and the empty state all derive from, so the
    // service is asked once per filter change instead of once per consumer. Cached on first access so
    // every derived feed shares the same instance.
    private IFeed<IImmutableList<ContactLocation>>? _filtered;
    private IFeed<IImmutableList<ContactLocation>> Filtered =>
        _filtered ??= Feed
            .Combine(SearchText, RegionFilter, SegmentFilter)
            .SelectAsync(async (criteria, ct) =>
                await Crm.SearchContactsAsync(criteria.Item1, criteria.Item2, criteria.Item3, ct));

    // The reactive source both the list and the map read from.
    public IListFeed<ContactLocation> FilteredContacts => Filtered.AsListFeed();

    // Header metrics. These project off Filtered — a SCALAR IFeed<IImmutableList<>>, not a list feed —
    // so an empty result is still Data and they read "0 contacts" rather than blank. (Lesson 94 bites
    // the list-feed form, not this one; keeping them on Filtered rather than on FilteredContacts is
    // what makes them safe, so don't "simplify" them onto the list feed.)
    public IFeed<string> TotalFilteredLabel =>
        Filtered.Select(list => $"{list.Count} contacts");

    public IFeed<string> RegionsLabel =>
        Filtered.Select(list => $"{DistinctCount(list, x => x.Region)} regions");

    public IFeed<string> SegmentsLabel =>
        Filtered.Select(list => $"{DistinctCount(list, x => x.Segment)} segments");

    private static int DistinctCount(IEnumerable<ContactLocation> items, Func<ContactLocation, string> selector)
        => items.Select(selector).Distinct(StringComparer.OrdinalIgnoreCase).Count();
}
