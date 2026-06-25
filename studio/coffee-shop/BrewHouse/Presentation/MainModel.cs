using BrewHouse.Presentation.MockData;
using BrewHouse.Presentation.Services;

namespace BrewHouse.Presentation;

// Backs the navigation shell (MainPage). Its only job is the live cart-count badge, derived from
// the shared cart so it stays current wherever an item is added/removed.
public partial record MainModel(ICartService Cart)
{
    // Item count = sum of line quantities. Derived from the one shared cart state, so the badge
    // updates from any page. The cart-as-list-feed goes to None when empty (no items), which is
    // exactly when the badge should be hidden — the shell renders this through a FeedView whose
    // NoneTemplate shows nothing.
    public IFeed<int> CartItemCount => Cart.Cart
        .AsFeed()
        .Select(items => items.Sum(i => i.Quantity));
}
