namespace BrewHouse.Presentation.MockData;

// Design-time DataContext for OrdersPage in Hot Design / Studio. The page renders the order book
// through a FeedView, and a FeedView can only subscribe to a FEED — so this mock is
// [ReactiveBindable] and its statics return the GENERATED ViewModel, whose constructor creates the
// SourceContext that makes the list pump. See MenuPageMockData for the full rationale; the same
// rules apply here (expression-bodied statics, never seeded from the page constructor).
[Uno.Extensions.Reactive.ReactiveBindable]
public partial record OrdersPageMockData
{
    // Default design-time state: the seeded order history.
    public static OrdersPageMockDataViewModel Data => new();

    // A second design-time state: no orders, so the feed emits an empty list and the FeedView falls
    // through to its NoneTemplate ("No orders yet"). This is the app's first-run state — reachable
    // in the running app only before the history loads, which is exactly why it needs a preview.
    public static OrdersPageMockDataViewModel Empty =>
        OrdersPageMockDataViewModel.ForModel(new()
        {
            Orders = ListFeed.Async(_ =>
                ValueTask.FromResult<IImmutableList<OrderRecord>>(ImmutableList<OrderRecord>.Empty)),
        });

    // A list FEED so it drives the page's FeedView the same way the runtime VM's IListState does.
    // Init-settable so the Empty variant above can supply no orders.
    public IListFeed<OrderRecord> Orders { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<OrderRecord>>(CatalogData.SeedOrders));
}

// The generator's model-taking ViewModel constructor is protected; this partial reaches it from
// inside the class so Empty can wrap a customized model.
public partial class OrdersPageMockDataViewModel
{
    internal static OrdersPageMockDataViewModel ForModel(OrdersPageMockData model) => new(model);
}
