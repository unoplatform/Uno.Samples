using BrewHouse.Presentation.MockData;
using BrewHouse.Presentation.Services;

namespace BrewHouse.Presentation;

// Order history. Reads the shared order book — a list STATE, because the app edits it (a freshly
// placed order is prepended by the cart) — whose initial contents are loaded from ICatalogService.
// The page renders it through a FeedView, so the first-run "no orders yet" case is the NoneTemplate
// and a failed history request is the ErrorTemplate.
public partial record OrdersModel(ICartService Cart)
{
    public IListState<OrderRecord> Orders => Cart.Orders;
}
