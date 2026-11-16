namespace Voyago.Presentation.MockData;

// The recipe every mock in this folder follows.
//
// Presentation/MockData holds one design-time DataContext per page, for Hot Design / Studio, and
// nothing else references them: the previews bind these statics in XAML, and no page constructor ever
// seeds one — see those constructors for why. HomePageMockData below is the reference; its siblings
// are built to the same four rules, recorded here once and cited from each of them.
//
// 1. The statics hand out the GENERATED ViewModel, never the record. Every page here renders at least
//    one FeedView, and a FeedView can only subscribe to a FEED — a plain list would never reach it and
//    the preview would sit on the empty state. So each mock is [ReactiveBindable], and the ViewModel
//    the analyzer emits for it creates, in its constructor, the SourceContext that makes those feeds
//    pump and materializes them into bindable lists. A raw record has no context, so its feeds never
//    emit.
//
// 2. Those statics are expression-bodied — a fresh instance per access, never a cached singleton. A
//    generated ViewModel has a view-scoped lifecycle: its SourceContext is created with the instance
//    and disposed when the hosting view unloads. A shared singleton can be built before the design
//    host's dispatcher is ready, or be already dead from a previous render, leaving feeds that never
//    emit. One instance per access mirrors the runtime per-navigation lifecycle.
//
// 3. Every static input is declared ABOVE the statics that construct instances, or initialized inline.
//    Static members initialize in textual order, and an instance initializer that reads a
//    not-yet-assigned static field gets null with no exception at all.
//
// 4. And the load-bearing one: every feed is built by an INLINE lambda that CAPTURES NOTHING — it
//    reads static seed data directly. A no-capture lambda's delegate is cached by the compiler, and
//    ListFeed.Async caches the feed it builds against that delegate instance, so every instance of a
//    mock shares ONE feed, created once. These feeds used to run through a MockFeeds.Of(params T[])
//    helper, whose lambda closed over its argument: that minted a fresh delegate, and so a fresh feed,
//    on every access — and the populated previews did not render. Do not "deduplicate" these lambdas
//    back into a helper; the repetition is the point.
//
// A variant needs the generated ViewModel's model-taking constructor, which is protected, so each file
// also carries a small partial on its ViewModel exposing a factory that reaches it.

/// <summary>
/// Design-time DataContext for <see cref="HomePage"/> in Hot Design / Studio, and the reference mock
/// for this folder — the recipe above applies to all of them. HomeModel's greeting and counts are
/// reactive members a design surface can't pump, so they are exposed here as plain materialized
/// values; the four carousels and strips stay list feeds because FeedViews render them. The generated
/// HomeModel VM replaces this whole mock at runtime.
/// </summary>
[ReactiveBindable]
public partial record HomePageMockData
{
    // Declared first: the statics below construct instances that read this (rule 3). The quick
    // actions exist nowhere else in the app's seed data, so this is their home.
    private static readonly IImmutableList<QuickAction> QuickActionSeed =
    [
        new("qa-01", "Flights"),
        new("qa-02", "Hotels"),
        new("qa-03", "Experiences"),
        new("qa-04", "Cars"),
        new("qa-05", "Trips"),
        new("qa-06", "Map"),
    ];

    /// <summary>The home screen as shipped: three hero destinations, all six quick actions.</summary>
    public static HomePageMockDataViewModel Data => new();

    public string GreetingText { get; init; } = "Where do you want to explore today?";
    public string UserInitials { get; init; } = "AJ";

    // Every feed below is an inline lambda that captures nothing — it reads Catalog or the seed field
    // above directly — so all instances of this mock share one feed (rule 4).
    public IListFeed<Destination> HeroDestinations { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<Destination>>(
            ImmutableList.Create(Catalog.Dolomites, Catalog.Maldives, Catalog.Kyoto)));

    public int HeroCount { get; init; } = 3;

    public IListFeed<QuickAction> QuickActions { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult(QuickActionSeed));

    public IListFeed<Destination> RecommendedTrips { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<Destination>>(
            ImmutableList.Create(Catalog.Santorini, Catalog.Bali, Catalog.Paris, Catalog.MachuPicchu)));

    public IListFeed<ExploreCategory> ExploreCategories { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<ExploreCategory>>(
            Catalog.Categories.ToImmutableList()));
}

// The MVUX analyzer generates HomePageMockDataViewModel for the [ReactiveBindable] mock above. Its
// public constructor always wraps a *default* model and its model-taking constructor is protected, so
// this partial adds a factory that reaches it from inside the class — which is how a variant (see the
// siblings' NoResults / FirstTrip / Empty) wraps a customized model.
public partial class HomePageMockDataViewModel
{
    internal static HomePageMockDataViewModel ForModel(HomePageMockData model) => new(model);
}
