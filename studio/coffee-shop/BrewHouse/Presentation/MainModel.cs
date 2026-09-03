using BrewHouse.Presentation.MockData;
using BrewHouse.Presentation.Services;

namespace BrewHouse.Presentation;

// Backs the navigation shell (MainPage). Its only job is the live cart-count badge, derived from
// the shared cart so it stays current wherever an item is added/removed.
public partial record MainModel(ICartService Cart)
{
    // Item count = sum of line quantities. Read off the shared, always-scalar cart summary, so the
    // badge text updates from any page and binds directly.
    public IFeed<int> CartItemCount => Cart.Summary.Select(summary => summary.ItemCount);

    // Whether the cart has anything in it — the badge is shown only when true, via a bool +
    // BoolToVisibility converter in XAML.
    public IFeed<bool> CartHasItems => Cart.Summary.Select(summary => summary.HasItems);
}
