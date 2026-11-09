using BrewHouse.Presentation.MockData;
using BrewHouse.Presentation.Services;

namespace BrewHouse.Presentation;

// The cart screen: editable line items (quantity +/-; a decrement at 1 drops the line), a live
// order summary (subtotal,
// 8% tax, total), and Place Order. All of it derives from the one shared cart state, so changes
// made here are reflected on Home and in the shell badge immediately. The order-confirmation toast
// is pure view behaviour and lives in the page code-behind, not here.
public partial record CartModel(ICartService Cart, INavigator Navigator)
{
    // The shared, mutable cart, bound directly to the items list. Whether it has anything in it is
    // surfaced as the CartHasItems bool below, which the page uses to swap in the empty-cart hero.
    public IListState<CartItem> CartItems => Cart.Cart;

    // Live summary (counts + money) — the shared, always-scalar projection off the cart state, so
    // the order-summary totals bind directly (e.g. {Binding Summary.TotalFormatted}) and update
    // reactively. Deriving the two scalars below from it (rather than re-projecting the cart state)
    // is what makes them render on an EMPTY cart: an empty list state emits None and Select() skips
    // None, so a projection straight off Cart.AsFeed() would leave the subtitle blank.
    public IFeed<CartSummary> Summary => Cart.Summary;

    // Header subtitle ("N items"); shows "0 items" when the cart is empty.
    public IFeed<string> ItemCountText => Cart.Summary.Select(summary => summary.ItemCountText);

    // Whether the cart has anything in it — chooses the body branch (items + summary vs. the
    // empty-cart hero) via a bool + BoolToVisibility converter in XAML.
    //
    // Deliberately NOT a FeedView, unlike the Menu and Orders pages. Those read the catalogue over
    // ICatalogService, so their feeds have genuine loading and failure states for a FeedView to
    // render. The cart is the user's own edited state — an IListState, which is what the MVUX docs
    // prescribe for a collection you edit — and it issues no request: there is nothing to be in
    // Progress or Error, and "empty" here is a product state with its own designed hero (suggestion
    // pills that navigate), not a data-availability state.
    public IFeed<bool> CartHasItems => Cart.Summary.Select(summary => summary.HasItems);

    // Quick "popular choices" shown in the empty-cart state; each pill jumps to the Menu.
    public IReadOnlyList<string> PopularChoices { get; } = ["Latte", "Croissant", "Matcha"];

    public async ValueTask Increment(CartItem item, CancellationToken ct)
        => await Cart.IncrementAsync(item.ProductId, ct);

    public async ValueTask Decrement(CartItem item, CancellationToken ct)
        => await Cart.DecrementAsync(item.ProductId, ct);

    public async ValueTask PlaceOrder(CancellationToken ct)
        => await Cart.PlaceOrderAsync(ct);

    public async ValueTask GoToMenu(CancellationToken ct)
        => await Navigator.NavigateRouteAsync(this, "Menu", cancellation: ct);
}
