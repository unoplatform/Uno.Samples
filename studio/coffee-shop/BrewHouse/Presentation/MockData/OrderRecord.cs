namespace BrewHouse.Presentation.MockData;

/// <summary>
/// A placed order, with the lines it was placed for. Carries <c>[property: Key]</c> on
/// <see cref="Id"/> because it lives in the shared CartService's <c>IListState&lt;OrderRecord&gt;</c>,
/// where an order is appended at checkout and its status updated afterwards — see
/// <see cref="ProductItem"/> for the convention.
/// </summary>
public partial record OrderRecord(
    [property: global::Uno.Extensions.Equality.Key] string Id,
    string PlacedAt,
    // A short status label ("Ready for Pickup", "Preparing", "Completed", "Confirmed"). The flags
    // below are data the XAML uses to pick the status indicator colour (the colours live in
    // App.xaml, not here).
    string Status,
    double Total,
    IImmutableList<OrderLineItem> Items)
{
    public bool IsReady => Status.Contains("Ready") || Status.Contains("Confirmed");
    public bool IsPreparing => Status.Contains("Preparing");
    public bool IsCompleted => !IsReady && !IsPreparing;
    public string TotalFormatted => Total.ToString("F2");

    public string ItemSummary =>
        Items is { Count: > 0 }
            ? string.Join(", ", Items.Select(i => $"{i.Name} x{i.Quantity}"))
            : "No items";

    // Builds an order from the current cart contents at checkout time.
    public static OrderRecord FromCart(string id, IEnumerable<CartItem> cart, double total) => new(
        Id: id,
        PlacedAt: DateTime.Now.ToString("MMM d, yyyy h:mm tt"),
        Status: "Confirmed",
        Total: total,
        Items: cart
            .Select(ci => new OrderLineItem(ci.Name, ci.Quantity, ci.Price, ci.ImageUrl))
            .ToImmutableList());
}
