namespace BrewHouse.Presentation.MockData;

// Design-time DataContext for OrdersPage (Hot Design / Studio preview). Mirrors OrdersModel's
// binding surface with representative data; at runtime the navigation-injected generated VM wins.
public partial record OrdersPageMockData
{
    public static OrdersPageMockData Data { get; } = new();

    // Plain, materialized list (not a feed) so the orders grid binds directly in Hot Design; the
    // live OrdersModel surfaces an IListState at runtime. HasNoOrders drives the empty state.
    public IReadOnlyList<OrderRecord> Orders => CatalogData.SeedOrders;
    public bool HasNoOrders => Orders.Count == 0;
}
