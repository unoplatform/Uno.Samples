using Voyago.Presentation.Services;

namespace Voyago.Presentation;

// The landing page. Everything is read from IDiscoveryService — asynchronously, like a real endpoint
// — so the greeting is a scalar feed that binds straight and the recommendations are a list feed the
// page renders through a FeedView.
public partial record HomeModel(IDiscoveryService Discovery)
{
    private IFeed<HomeGreeting>? _greeting;
    private IFeed<HomeGreeting> Greeting => _greeting ??= Feed.Async(Discovery.GetGreetingAsync);

    public IFeed<string> GreetingText => Greeting.Select(g => g.GreetingText);
    public IFeed<string> UserInitials => Greeting.Select(g => g.UserInitials);

    // The hero carousel. Bound straight to the FlipView's ItemsSource rather than through a FeedView:
    // its paging is driven from code-behind by page-scope x:Name (Hero / HeroPager), which a
    // DataTemplate's namescope would hide — and a promo carousel's empty state is simply no carousel.
    private IListFeed<Destination>? _hero;
    public IListFeed<Destination> HeroDestinations =>
        _hero ??= ListFeed.Async(Discovery.GetHeroDestinationsAsync);

    // The PipsPager's NumberOfPages. SelectData, not Select: an empty list feed emits None and Select
    // skips None, which would leave the pager on its default of 5 phantom pips (lesson 49's symptom,
    // via lesson 94's cause).
    public IFeed<int> HeroCount =>
        HeroDestinations
            .AsFeed()
            .SelectData<IImmutableList<Destination>, int>(items => items.SomeOrDefault()?.Count ?? 0);

    // Navigation chrome — bound directly; "no quick actions" has no UI worth designing.
    private IListFeed<QuickAction>? _quickActions;
    public IListFeed<QuickAction> QuickActions =>
        _quickActions ??= ListFeed.Async(Discovery.GetQuickActionsAsync);

    // The page's primary content section, so it gets a FeedView.
    private IListFeed<Destination>? _recommended;
    public IListFeed<Destination> RecommendedTrips =>
        _recommended ??= ListFeed.Async(Discovery.GetRecommendedAsync);

    // Shared with Search — each tile opens its Featured destination when tapped. Chrome, so direct.
    private IListFeed<ExploreCategory>? _categories;
    public IListFeed<ExploreCategory> ExploreCategories =>
        _categories ??= ListFeed.Async(Discovery.GetCategoriesAsync);
}

// Page-local record — QuickAction is only used on HomePage
public partial record QuickAction(string Id, string Label);
