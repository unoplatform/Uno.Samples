namespace BrewHouse.Presentation.MockData;

// Design-time data for the Hot Design *component* previews (the product card and the order status
// badge), referenced from the preview XAML via {x:Bind} so no preview needs code-behind. Page
// previews bind their own *PageMockData.Data statics directly; this class only holds the individual
// entities the DataTemplate previews render.
public static class PreviewData
{
    // Product card
    public static ProductItem SpecialProduct => CatalogData.AllProducts[0];  // Classic Latte (special)
    public static ProductItem StandardProduct => CatalogData.AllProducts[1]; // Cappuccino (not special)

    // A deliberately long name + description, to show the card trimming overflowing text.
    public static ProductItem LongNameProduct { get; } = new(
        "preview-long",
        "Extra-Hot Triple-Shot Oat-Milk Caramel Macchiato with Vanilla Cold Foam",
        "A deliberately long description to show how the card trims overflowing text to a single line with an ellipsis.",
        "Hot Drinks", "hot", "7.25", 7.25,
        CatalogData.AllProducts[0].ImageUrl,
        IsFeatured: false, IsSpecial: true);

    // Order status badge
    public static OrderRecord ReadyOrder => CatalogData.SeedOrders[0];     // "Ready for Pickup"
    public static OrderRecord PreparingOrder => CatalogData.SeedOrders[1]; // "Preparing"
    public static OrderRecord CompletedOrder => CatalogData.SeedOrders[2]; // "Completed"
}
