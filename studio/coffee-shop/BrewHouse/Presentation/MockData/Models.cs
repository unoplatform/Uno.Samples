namespace BrewHouse.Presentation.MockData;

// All entities are immutable records (MVUX requirement). Records used inside an IListState<T>
// declare a [property: Key] so add/update/remove operations and selection match the right item
// by identity rather than by reference (key equality is auto-generated for partial records).

public partial record HeroBanner(
    string ImageUrl,
    string Title,
    string Subtitle);

public partial record ProductItem(
    [property: global::Uno.Extensions.Equality.Key] string Id,
    string Name,
    string Description,
    string Category,
    string CategoryId,
    string Price,
    double PriceValue,
    string ImageUrl,
    bool IsFeatured,
    bool IsSpecial);

public partial record CategoryItem(
    string Id,
    string Name,
    // Drives the filter chip's selected look in XAML (theme brushes), not a hardcoded colour.
    bool IsSelected = false);

public partial record CartItem(
    [property: global::Uno.Extensions.Equality.Key] string ProductId,
    string Name,
    string ImageUrl,
    double Price,
    int Quantity)
{
    public double LineTotal => Price * Quantity;
    public string LineTotalFormatted => LineTotal.ToString("F2");
    public string PriceFormatted => Price.ToString("F2");
}

public partial record OrderLineItem(
    string Name,
    int Quantity,
    double Price,
    string ImageUrl = "");

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
