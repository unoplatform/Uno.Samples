using BrewHouse.Presentation.MockData;
using BrewHouse.Presentation.Services;

namespace BrewHouse.Presentation;

// Backs the navigation shell (MainPage). Its only job is the live cart-count badge, derived from
// the shared cart so it stays current wherever an item is added/removed.
public partial record MainModel(ICartService Cart)
{
    // Item count = sum of line quantities. Derived from the one shared cart state, so the badge text
    // updates from any page. A scalar projection (never None) so it binds directly to the badge.
    public IFeed<int> CartItemCount => Cart.Cart
        .AsFeed()
        .Select(items => items.Sum(i => i.Quantity));

    // Whether the cart has anything in it — the badge is shown only when true, via a bool +
    // BoolToVisibility converter in XAML. Scalar projection so it flips reliably.
    public IFeed<bool> CartHasItems => Cart.Cart
        .AsFeed()
        .Select(items => items.Sum(i => i.Quantity) > 0);
}
