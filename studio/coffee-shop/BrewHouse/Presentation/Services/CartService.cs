using BrewHouse.Presentation.MockData;

namespace BrewHouse.Presentation.Services;

// One shared cart + order book for the whole app. Registered as a DI singleton and injected into
// every page-Model, so the SAME IListState<CartItem>/IListState<OrderRecord> instances back every
// screen — a mutation here propagates to every feed derived from them, with no messaging or manual
// PropertyChanged fan-out.
// A read-only summary of the cart at a moment in time, derived from the shared cart state. Carries
// only data (counts, money, formatted strings) — the view decides how to render it.
public partial record CartSummary(IImmutableList<CartItem> Items)
{
    public int ItemCount => Items.Sum(i => i.Quantity);
    public bool HasItems => ItemCount > 0;
    public string ItemCountText => ItemCount == 1 ? "1 item" : $"{ItemCount} items";

    public double Subtotal => Items.Sum(i => i.LineTotal);
    public double Tax => Math.Round(Subtotal * 0.08, 2);
    public double Total => Subtotal + Tax;

    public string SubtotalFormatted => Subtotal.ToString("F2");
    public string TaxFormatted => Tax.ToString("F2");
    public string TotalFormatted => Total.ToString("F2");
}

public interface ICartService
{
    // The shared, mutable cart and order book. Pages derive read-only feeds (totals, counts,
    // filtered views) from these.
    IListState<CartItem> Cart { get; }
    IListState<OrderRecord> Orders { get; }

    // The immutable product catalogue (Home/Menu/ProductDetail all read from this).
    IReadOnlyList<ProductItem> AllProducts { get; }

    ValueTask AddToCartAsync(ProductItem product, CancellationToken ct = default);
    ValueTask IncrementAsync(string productId, CancellationToken ct = default);
    ValueTask DecrementAsync(string productId, CancellationToken ct = default);
    ValueTask RemoveAsync(string productId, CancellationToken ct = default);
    ValueTask PlaceOrderAsync(CancellationToken ct = default);
}

public sealed class CartService : ICartService
{
    // CRITICAL: Cart/Orders are once-initialized (assigned in the ctor), NOT expression-bodied
    // getters — a `=>` getter would rebuild a fresh state on every access and break sharing. The
    // service itself is the reactive owner; as a singleton it lives for the app's lifetime.
    public IListState<CartItem> Cart { get; }
    public IListState<OrderRecord> Orders { get; }

    public IReadOnlyList<ProductItem> AllProducts => CatalogData.AllProducts;

    public CartService()
    {
        Cart = ListState<CartItem>.Empty(this);
        Orders = ListState.Value(this, () => CatalogData.SeedOrders);
    }

    // Add a product: merge into the existing line (quantity++) if it's already in the cart (matched
    // by ProductId key), otherwise append a new line. The whole-list updater is pure.
    public async ValueTask AddToCartAsync(ProductItem product, CancellationToken ct = default)
    {
        await Cart.Update(existing =>
        {
            existing ??= ImmutableList<CartItem>.Empty;
            var match = existing.FirstOrDefault(i => i.ProductId == product.Id);
            return match is not null
                ? existing.Replace(match, match with { Quantity = match.Quantity + 1 })
                : existing.Add(new CartItem(product.Id, product.Name, product.ImageUrl, product.PriceValue, 1));
        }, ct);
    }

    public async ValueTask IncrementAsync(string productId, CancellationToken ct = default)
        => await Cart.UpdateAllAsync(
            match: i => i.ProductId == productId,
            updater: i => i with { Quantity = i.Quantity + 1 },
            ct);

    // Decrement, then drop any line that fell to zero (decrementing at qty 1 removes the line).
    public async ValueTask DecrementAsync(string productId, CancellationToken ct = default)
    {
        await Cart.UpdateAllAsync(
            match: i => i.ProductId == productId,
            updater: i => i with { Quantity = i.Quantity - 1 },
            ct);
        await Cart.RemoveAllAsync(i => i.Quantity <= 0, ct);
    }

    public async ValueTask RemoveAsync(string productId, CancellationToken ct = default)
        => await Cart.RemoveAllAsync(i => i.ProductId == productId, ct);

    // Snapshot the cart into a new order, prepend it to the order book, then clear the cart.
    public async ValueTask PlaceOrderAsync(CancellationToken ct = default)
    {
        var items = await Cart.Value(ct);
        if (items is not { Count: > 0 })
        {
            return;
        }

        var total = items.Sum(i => i.LineTotal);
        var existingOrders = await Orders.Value(ct);
        var orderNumber = 1042 + (existingOrders?.Count ?? 0) + 1;
        var order = OrderRecord.FromCart($"ORD-{orderNumber:D4}", items, total);

        await Orders.InsertAsync(order, ct);
        await Cart.RemoveAllAsync(_ => true, ct);
    }
}
