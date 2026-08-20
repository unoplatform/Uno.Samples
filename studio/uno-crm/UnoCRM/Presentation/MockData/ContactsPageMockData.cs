namespace UnoCRM.Presentation.MockData;

/// <summary>
/// Design-time DataContext for <see cref="ContactsPage"/> in Hot Design / Studio. ContactsModel has
/// reactive members (states + a list feed) that the design surface can't pump, so
/// the mock exposes plain, materialized values (settable strings for the two-way filters, plain
/// lists, a plain bool) instead. The generated ContactsModel VM overrides this at runtime.
/// </summary>
public partial record ContactsPageMockData
{
    public static ContactsPageMockData Data { get; } = new();

    public string SearchText { get; set; } = string.Empty;
    public string RegionFilter { get; set; } = ContactsModel.AllRegions;
    public string SegmentFilter { get; set; } = ContactsModel.AllSegments;

    public IReadOnlyList<string> Regions { get; } =
        new[] { ContactsModel.AllRegions }
            .Concat(CrmData.Contacts.Select(c => c.Region).Distinct(StringComparer.OrdinalIgnoreCase))
            .ToArray();

    public IReadOnlyList<string> Segments { get; } =
        new[] { ContactsModel.AllSegments }
            .Concat(CrmData.Contacts.Select(c => c.Segment).Distinct(StringComparer.OrdinalIgnoreCase))
            .ToArray();

    public IReadOnlyList<ContactLocation> FilteredContacts { get; } = CrmData.Contacts;

    public string TotalFilteredLabel => $"{CrmData.Contacts.Count} contacts";
    public string RegionsLabel => $"{CrmData.Contacts.Select(c => c.Region).Distinct().Count()} regions";
    public string SegmentsLabel => $"{CrmData.Contacts.Select(c => c.Segment).Distinct().Count()} segments";
    public bool HasNoResults => false;
}
