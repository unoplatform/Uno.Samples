namespace UnoCRM.Presentation.MockData;

// The recipe every mock in this folder follows.
//
// Presentation/MockData holds one design-time DataContext per page, for Hot Design / Studio.
// ContactsPageMockData below is the reference: it is the mock whose populated preview provably
// renders on a design surface, so its siblings are built to the same four rules. They are recorded
// here once, and each sibling file points back at them.
//
// 1. The statics hand out the GENERATED ViewModel, never the record. Every page renders at least one
//    FeedView, and a FeedView can only subscribe to a FEED — a plain list would never reach it. So
//    each mock is [ReactiveBindable], and the ViewModel the analyzer emits for it creates, in its
//    constructor, the SourceContext that makes those feeds pump and materializes them into bindable
//    lists. A raw record has no context, so its feeds never emit and the preview comes out empty.
//    Handing the FeedView the bare feed instead of the ViewModel was tried, and does not help.
//
// 2. Those statics are expression-bodied — a fresh instance per access, never a cached singleton. A
//    generated ViewModel has a view-scoped lifecycle: its SourceContext is created with the instance
//    and disposed when the hosting view unloads. A shared singleton can be built before the design
//    host's dispatcher is ready, or be disposed after a previous preview render, leaving a dead
//    context whose feeds never emit. One instance per access mirrors the runtime per-navigation
//    lifecycle, so every preview render gets a live ViewModel.
//
// 3. Every static input is declared ABOVE the statics that construct instances. Static members
//    initialize in textual order, and an instance initializer that reads a not-yet-assigned static
//    field gets null with no exception at all.
//
// 4. And the load-bearing one: every feed is built by an INLINE lambda that CAPTURES NOTHING — it
//    reads static seed data directly. A no-capture lambda's delegate is cached by the compiler, and
//    the feed factories cache the feed they build against that delegate instance, so every instance
//    of a mock shares ONE feed, created once. Routing these through a shared helper that hoisted the
//    payload into a local minted a fresh delegate, and so a fresh feed, per mock instance — and
//    those previews rendered their lists empty. Do not "deduplicate" these lambdas into a helper.
//
// A variant needs the generated ViewModel's model-taking constructor, which is protected, so each
// file also carries a small partial on its ViewModel exposing a factory that reaches it.
//
// None of these is ever seeded from a page constructor — see those constructors for why.

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
