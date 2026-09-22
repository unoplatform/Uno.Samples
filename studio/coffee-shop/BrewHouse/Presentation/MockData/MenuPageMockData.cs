namespace BrewHouse.Presentation.MockData;

// Design-time DataContext for MenuPage in Hot Design / Studio. MenuPage renders its products through
// a FeedView, and a FeedView can only subscribe to a FEED — a plain list would never reach it and the
// preview would sit on the None state. So this mock is [ReactiveBindable] and its statics return the
// GENERATED ViewModel, whose constructor creates the SourceContext that makes FilteredProducts pump.
//
// Expression-bodied (a fresh instance per access), never a cached `{ get; } = new()` singleton: a
// generated ViewModel has a view-scoped lifecycle — its SourceContext is created with the instance
// and disposed when the hosting view unloads — so a shared instance can be built before Hot Design's
// dispatcher is ready, or be dead from a previous render, leaving a feed that never emits.
//
// This is the ONLY place a generated ViewModel may be constructed by hand. It must never be seeded
// from the page constructor: a hand-built VM has no live context at runtime and would shadow the
// navigation-injected one.
[Uno.Extensions.Reactive.ReactiveBindable]
public partial record MenuPageMockData
{
    // Default design-time state: the whole catalogue.
    public static MenuPageMockDataViewModel Data => new();

    // A second design-time state: a search that matches nothing, so FilteredProducts emits an empty
    // list and the FeedView falls through to its NoneTemplate ("No matches found"). Wraps a
    // customized model via the factory below, because the generator's model-taking constructor is
    // protected.
    public static MenuPageMockDataViewModel NoResults =>
        MenuPageMockDataViewModel.ForModel(new()
        {
            SearchText = "unicorn frappé",
            FilteredProducts = ListFeed.Async(_ =>
                ValueTask.FromResult<IImmutableList<ProductItem>>(ImmutableList<ProductItem>.Empty)),
        });

    // Plain settable values so the search box two-way binds at design time.
    public string SearchText { get; set; } = string.Empty;
    public string CategoryId { get; set; } = "all";

    // The chip bar is bound directly (not through a FeedView), but mirror the Model's type so the
    // generated ViewModel exposes the same bindable shape.
    public IListFeed<CategoryItem> Categories { get; init; } =
        ListFeed.Async(_ =>
            ValueTask.FromResult<IImmutableList<CategoryItem>>(CatalogData.Categories.ToImmutableList()));

    // A list FEED, so it drives the page's FeedView exactly as the runtime VM's IListFeed does.
    // ListFeed.Async emits immediately and Hot Design is a live running app, so the FeedView
    // subscribes and inflates its ValueTemplate at design time. Init-settable so the NoResults
    // variant above can supply an empty set.
    public IListFeed<ProductItem> FilteredProducts { get; init; } =
        ListFeed.Async(_ =>
            ValueTask.FromResult<IImmutableList<ProductItem>>(CatalogData.AllProducts.ToImmutableList()));

    public void AddToCart(ProductItem product) { }
    public void ViewProduct(ProductItem product) { }
    public void FilterByCategory(string categoryId) { }
}

// The MVUX generator emits MenuPageMockDataViewModel for the [ReactiveBindable] mock above. Its
// public constructor always wraps a *default* model and its model-taking constructor is protected —
// so this partial adds a factory that reaches it from inside the class, letting NoResults wrap a
// customized (empty-result) model.
public partial class MenuPageMockDataViewModel
{
    internal static MenuPageMockDataViewModel ForModel(MenuPageMockData model) => new(model);
}
