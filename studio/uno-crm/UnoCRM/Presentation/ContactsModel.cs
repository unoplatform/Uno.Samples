namespace UnoCRM.Presentation;

/// <summary>
/// Backs <see cref="ContactsPage"/>. The three filters (search text, region, segment) are two-way
/// <see cref="IState{T}"/> — the desktop and mobile controls bind the SAME state, so they stay in
/// lockstep for free (no manual mirror / sync flag). The filtered list and the header counts are
/// feeds derived from those states via <see cref="Feed"/>.<c>Combine</c>, so they recompute
/// reactively; the page code-behind observes the list and re-renders the Mapsui map.
/// </summary>
public partial record ContactsModel
{
    public const string AllRegions = "All Regions";
    public const string AllSegments = "All Segments";

    // Two-way mutable user input. One state each, bound by both layouts' controls.
    public IState<string> SearchText => State.Value(this, () => string.Empty);
    public IState<string> RegionFilter => State.Value(this, () => AllRegions);
    public IState<string> SegmentFilter => State.Value(this, () => AllSegments);

    // Filter options derived from the data (not hardcoded), with an "All …" entry first.
    public IReadOnlyList<string> Regions { get; } =
        new[] { AllRegions }
            .Concat(CrmData.Contacts.Select(c => c.Region).Distinct(StringComparer.OrdinalIgnoreCase))
            .ToArray();

    public IReadOnlyList<string> Segments { get; } =
        new[] { AllSegments }
            .Concat(CrmData.Contacts.Select(c => c.Segment).Distinct(StringComparer.OrdinalIgnoreCase))
            .ToArray();

    // The single reactive source both the list and the map read from.
    public IListFeed<ContactLocation> FilteredContacts =>
        Feed.Combine(SearchText, RegionFilter, SegmentFilter)
            .Select(c => (IImmutableList<ContactLocation>)Filter(c.Item1, c.Item2, c.Item3))
            .AsListFeed();

    // Header metrics as scalar feeds (never None), so they bind directly — no INPC.
    public IFeed<string> TotalFilteredLabel =>
        Feed.Combine(SearchText, RegionFilter, SegmentFilter)
            .Select(c => $"{Filter(c.Item1, c.Item2, c.Item3).Count} contacts");

    public IFeed<string> RegionsLabel =>
        Feed.Combine(SearchText, RegionFilter, SegmentFilter)
            .Select(c => $"{DistinctCount(Filter(c.Item1, c.Item2, c.Item3), x => x.Region)} regions");

    public IFeed<string> SegmentsLabel =>
        Feed.Combine(SearchText, RegionFilter, SegmentFilter)
            .Select(c => $"{DistinctCount(Filter(c.Item1, c.Item2, c.Item3), x => x.Segment)} segments");

    // Empty-state flag: a bool feed the XAML shows/hides an empty message with (via BoolToVisibility).
    public IFeed<bool> HasNoResults =>
        Feed.Combine(SearchText, RegionFilter, SegmentFilter)
            .Select(c => Filter(c.Item1, c.Item2, c.Item3).Count == 0);

    private IImmutableList<ContactLocation> Filter(string? search, string? region, string? segment)
    {
        var query = (search ?? string.Empty).Trim();
        var reg = region ?? AllRegions;
        var seg = segment ?? AllSegments;

        return CrmData.Contacts
            .Where(x => reg == AllRegions || x.Region.Equals(reg, StringComparison.OrdinalIgnoreCase))
            .Where(x => seg == AllSegments || x.Segment.Equals(seg, StringComparison.OrdinalIgnoreCase))
            .Where(x => query.Length == 0
                        || x.Name.Contains(query, StringComparison.OrdinalIgnoreCase)
                        || x.Company.Contains(query, StringComparison.OrdinalIgnoreCase)
                        || x.City.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToImmutableList();
    }

    private static int DistinctCount(IEnumerable<ContactLocation> items, Func<ContactLocation, string> selector)
        => items.Select(selector).Distinct(StringComparer.OrdinalIgnoreCase).Count();
}
