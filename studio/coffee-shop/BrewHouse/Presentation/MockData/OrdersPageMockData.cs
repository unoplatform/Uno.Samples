namespace BrewHouse.Presentation.MockData;

// Design-time DataContext for OrdersPage (Hot Design / Studio preview). Mirrors OrdersModel's
// binding surface with representative data; at runtime the navigation-injected generated VM wins.
public partial record OrdersPageMockData
{
    // Default design-time state: the seed order history.
    public static OrdersPageMockData Data { get; } = new();

    // A second design-time state: no orders, so the "No orders yet" card shows. The
    // "Orders — Empty" preview uses this to demonstrate previewing the same page with no data.
    public static OrdersPageMockData Empty { get; } = new() { Orders = [] };

    // Plain, materialized list (not a feed) so the orders grid binds directly in Hot Design; the
    // live OrdersModel surfaces an IListState at runtime. Init-settable so a variant (see Empty) can
    // supply no orders; defaults to the seed history. HasNoOrders (computed) drives the empty state.
    public IReadOnlyList<OrderRecord> Orders { get; init; } = CatalogData.SeedOrders;
    public bool HasNoOrders => Orders.Count == 0;
}
