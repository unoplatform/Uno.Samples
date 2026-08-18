namespace UnoCRM.Presentation.MockData;

/// <summary>
/// Design-time DataContext for <see cref="ContactsPage"/> in Hot Design / Studio. ContactsModel has
/// reactive members (states + a list feed) that the design surface can't pump, so
/// the mock exposes plain, materialized values (settable strings for the two-way filters, plain
/// lists, a plain bool) instead. The generated ContactsModel VM overrides this at runtime.
/// </summary>
public partial record ContactsPageMockData
{
    // Default design-time state: all contacts, no filter.
    public static ContactsPageMockData Data { get; } = new();

    // A second design-time state: a search that matches nothing, so the "No contacts match your
    // filters." message shows and the list/map are empty. Used by the "Contacts — No Results" preview.
    public static ContactsPageMockData NoResults { get; } = new()
    {
        SearchText = "zzz",
        FilteredContacts = Array.Empty<ContactLocation>(),
        TotalFilteredLabel = "0 contacts",
        RegionsLabel = "0 regions",
        SegmentsLabel = "0 segments",
        HasNoResults = true,
    };

    public string SearchText { get; set; } = string.Empty;
    public string RegionFilter { get; set; } = ContactsModel.AllRegions;
    public string SegmentFilter { get; set; } = ContactsModel.AllSegments;

    // Dropdown option lists are filter-independent, so they stay computed off the full set.
    public IReadOnlyList<string> Regions { get; } =
        new[] { ContactsModel.AllRegions }
            .Concat(CrmData.Contacts.Select(c => c.Region).Distinct(StringComparer.OrdinalIgnoreCase))
            .ToArray();

    public IReadOnlyList<string> Segments { get; } =
        new[] { ContactsModel.AllSegments }
            .Concat(CrmData.Contacts.Select(c => c.Segment).Distinct(StringComparer.OrdinalIgnoreCase))
            .ToArray();

    // Init-settable so a variant (see NoResults) can supply an empty/filtered set; default to the
    // full catalogue with matching header counts.
    public IReadOnlyList<ContactLocation> FilteredContacts { get; init; } = CrmData.Contacts;

    public string TotalFilteredLabel { get; init; } = $"{CrmData.Contacts.Count} contacts";
    public string RegionsLabel { get; init; } = $"{CrmData.Contacts.Select(c => c.Region).Distinct().Count()} regions";
    public string SegmentsLabel { get; init; } = $"{CrmData.Contacts.Select(c => c.Segment).Distinct().Count()} segments";
    public bool HasNoResults { get; init; }
}
