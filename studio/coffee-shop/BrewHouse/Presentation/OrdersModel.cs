using BrewHouse.Presentation.MockData;
using BrewHouse.Presentation.Services;

namespace BrewHouse.Presentation;

// Order history. Reads the shared order book; a freshly placed order (prepended by the cart) shows
// up here live. The list binds directly as an ItemsSource; an empty-state card shows when there are
// no orders, driven by a bool feed + BoolToVisibility converter.
public partial record OrdersModel(ICartService Cart)
{
    public IListState<OrderRecord> Orders => Cart.Orders;

    // True when there are no orders — drives the empty-state card. Scalar projection (never None)
    // so it flips reliably even at zero orders.
    public IFeed<bool> HasNoOrders => Cart.Orders
        .AsFeed()
        .Select(orders => orders.Count == 0);
}
