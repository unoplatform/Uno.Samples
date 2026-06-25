using BrewHouse.Presentation.MockData;
using BrewHouse.Presentation.Services;

namespace BrewHouse.Presentation;

// Order history. Reads the shared order book; a freshly placed order (prepended by the cart) shows
// up here live. Goes to None when there are no orders, which the page renders as an empty state.
public partial record OrdersModel(ICartService Cart)
{
    public IListState<OrderRecord> Orders => Cart.Orders;
}
