namespace BrewHouse.Presentation.MockData;

// Design-time DataContext for OrdersPage (Hot Design / Studio preview). Mirrors OrdersModel's
// binding surface with representative data; at runtime the navigation-injected generated VM wins.
public partial record OrdersPageMockData
{
    public static OrdersPageMockData Data { get; } = new();

    // FeedView binds to this; ListFeed.Async over the seed orders renders the value template.
    public IListFeed<OrderRecord> Orders => ListFeed.Async(
        async _ => CatalogData.SeedOrders);
}
