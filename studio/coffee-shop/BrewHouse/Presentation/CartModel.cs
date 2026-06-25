using BrewHouse.Presentation.MockData;
using BrewHouse.Presentation.Services;

namespace BrewHouse.Presentation;

// The cart screen: editable line items (quantity +/-, remove), a live order summary (subtotal,
// 8% tax, total), and Place Order. All of it derives from the one shared cart state, so changes
// made here are reflected on Home and in the shell badge immediately. The order-confirmation toast
// is pure view behaviour and lives in the page code-behind, not here (lesson 28).
public partial record CartModel(ICartService Cart, INavigator Navigator)
{
    // The shared, mutable cart. Bound directly to the items list; goes to None when empty, which the
    // page renders as the empty-cart hero via FeedView.
    public IListState<CartItem> CartItems => Cart.Cart;

    // Live summary (counts + money). None when the cart is empty.
    public IFeed<CartSummary> Summary => Cart.Cart
        .AsFeed()
        .Select(items => new CartSummary(items));

    // Header subtitle ("N items"). Scalar projection so it shows "0 items" when the cart is empty
    // (the summary feed itself goes to None then, but the header is always visible).
    public IFeed<string> ItemCountText => Cart.Cart
        .AsFeed()
        .Select(items => new CartSummary(items).ItemCountText);

    public async ValueTask Increment(CartItem item, CancellationToken ct)
        => await Cart.IncrementAsync(item.ProductId, ct);

    public async ValueTask Decrement(CartItem item, CancellationToken ct)
        => await Cart.DecrementAsync(item.ProductId, ct);

    public async ValueTask RemoveItem(CartItem item, CancellationToken ct)
        => await Cart.RemoveAsync(item.ProductId, ct);

    public async ValueTask PlaceOrder(CancellationToken ct)
        => await Cart.PlaceOrderAsync(ct);

    public async ValueTask GoToMenu(CancellationToken ct)
        => await Navigator.NavigateRouteAsync(this, "Menu", cancellation: ct);
}
