namespace UnoCRM.Presentation.MockData;

/// <summary>
/// Design-time DataContext for <see cref="ContactsPage"/> in Hot Design / Studio. ContactsModel has
/// reactive members the design surface can't pump, so the two-way filters and header counts are
/// exposed as plain, materialized values instead. The one exception is <see cref="FilteredContacts"/>:
/// the page's <c>mvux:FeedView</c> binds its <c>Source</c> to it, and a FeedView can only subscribe
/// to a feed — so this stays a list feed (see below) rather than a plain list. The generated
/// ContactsModel VM overrides this whole mock at runtime.
/// </summary>
[ReactiveBindable]
public partial record ContactsPageMockData
{
    // Default design-time state: all contacts, no filter. Backs the "All Contacts" preview.
    // These return the generated MVUX ViewModel (not the raw model): the ViewModel's constructor
    // creates a SourceContext and materializes FilteredContacts into a bindable list, which is what
    // lets the page's FeedView render at design time. A raw model has no context, so its list feed
    // never pumps and the preview would be empty.
    //
    // Expression-bodied (fresh instance per access), NOT a cached singleton: a generated ViewModel
    // has a view-scoped lifecycle — its SourceContext is created with the instance and disposed when
    // the hosting view unloads. A shared singleton can be created before Hot Design's dispatcher is
    // ready, or be disposed after a previous preview render, leaving a dead context whose feed never
    // emits (empty list). Building one per access mirrors the runtime per-navigation lifecycle, so
    // every preview render gets a live ViewModel.
    public static ContactsPageMockDataViewModel Data => new();

    // A second design-time state: a search that matches nothing, so FilteredContacts emits an empty
    // list and the FeedView falls through to its NoneTemplate ("No contacts match your filters.").
    // Backs the "No Results" preview. Wraps a customized model via the ViewModel factory below,
    // because the generator's model-taking ViewModel constructor is protected.
    public static ContactsPageMockDataViewModel NoResults =>
        ContactsPageMockDataViewModel.ForModel(new()
        {
            SearchText = "zzz",
            FilteredContacts = ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<ContactLocation>>(ImmutableList<ContactLocation>.Empty)),
            TotalFilteredLabel = "0 contacts",
            RegionsLabel = "0 regions",
            SegmentsLabel = "0 segments",
        });

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

    // A list FEED (not a plain list) so it drives the page's FeedView the same way the runtime VM's
    // IListFeed<ContactLocation> does. ListFeed.Async emits immediately, and Hot Design / Studio is a
    // live running app, so the FeedView subscribes and renders the cards via its ValueTemplate at
    // design time. A plain list here would never reach the FeedView and the preview would be empty.
    // Init-settable so a variant (see NoResults) can supply an empty set; defaults to the full catalogue.
    public IListFeed<ContactLocation> FilteredContacts { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<ContactLocation>>(CrmData.Contacts.ToImmutableList()));

    public string TotalFilteredLabel { get; init; } = $"{CrmData.Contacts.Count} contacts";
    public string RegionsLabel { get; init; } = $"{CrmData.Contacts.Select(c => c.Region).Distinct().Count()} regions";
    public string SegmentsLabel { get; init; } = $"{CrmData.Contacts.Select(c => c.Segment).Distinct().Count()} segments";
}

// The MVUX analyzer generates ContactsPageMockDataViewModel (a BindableViewModelBase) for the
// [ReactiveBindable] mock above. Its public constructor always wraps a *default* model, and its
// model-taking constructor is protected — so this partial adds a factory that reaches that
// constructor from inside the class, letting NoResults wrap a customized (empty-set) model.
public partial class ContactsPageMockDataViewModel
{
    internal static ContactsPageMockDataViewModel ForModel(ContactsPageMockData model) => new(model);
}
