namespace BrewHouse.Presentation.MockData;

// Immutable seed data for the shop: the product catalogue, the category filter set, and a little
// order history so the Orders tab has content on first run. No INPC, no mutation — pages and the
// CartService read from here and project into feeds/states.
public static class CatalogData
{
    public static IReadOnlyList<ProductItem> AllProducts { get; } =
    [
        new("p-001", "Classic Latte",
            "Smooth espresso with velvety steamed milk and a delicate foam layer.",
            "Hot Drinks", "hot", "5.50", 5.50,
            "https://images.pexels.com/photos/3646111/pexels-photo-3646111.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            IsFeatured: true, IsSpecial: true),
        new("p-002", "Cappuccino",
            "Equal parts espresso, steamed milk and thick microfoam — a true Italian classic.",
            "Hot Drinks", "hot", "4.75", 4.75,
            "https://images.pexels.com/photos/1694874/pexels-photo-1694874.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            IsFeatured: true, IsSpecial: false),
        new("p-003", "Butter Croissant",
            "Flaky, golden-baked croissant with premium French butter. Freshly baked each morning.",
            "Pastries", "pastries", "3.25", 3.25,
            "https://images.pexels.com/photos/20212456/pexels-photo-20212456.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            IsFeatured: true, IsSpecial: true),
        new("p-004", "Iced Matcha",
            "Premium ceremonial matcha blended with oat milk over crushed ice.",
            "Cold Drinks", "cold", "6.00", 6.00,
            "https://images.pexels.com/photos/18553404/pexels-photo-18553404.png?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            IsFeatured: true, IsSpecial: false),
        new("p-005", "Flat White",
            "Ristretto shots with a thin layer of velvety micro-foam — bold and smooth.",
            "Hot Drinks", "hot", "4.50", 4.50,
            "https://images.pexels.com/photos/131053/pexels-photo-131053.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            IsFeatured: false, IsSpecial: false),
        new("p-006", "Cold Brew",
            "Steeped for 18 hours in cold water for a naturally sweet, low-acid brew.",
            "Cold Drinks", "cold", "5.00", 5.00,
            "https://images.pexels.com/photos/261434/pexels-photo-261434.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            IsFeatured: false, IsSpecial: false),
        new("p-007", "Almond Muffin",
            "Moist almond muffin with a crunchy streusel topping and hint of vanilla.",
            "Pastries", "pastries", "2.95", 2.95,
            "https://images.pexels.com/photos/20212456/pexels-photo-20212456.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            IsFeatured: false, IsSpecial: false),
        new("p-008", "Caramel Macchiato",
            "Layers of vanilla syrup, steamed milk, espresso and drizzled caramel.",
            "Hot Drinks", "hot", "5.75", 5.75,
            "https://images.pexels.com/photos/3646111/pexels-photo-3646111.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            IsFeatured: false, IsSpecial: true),
    ];

    public static IReadOnlyList<HeroBanner> HeroBanners { get; } =
    [
        new("https://images.pexels.com/photos/1002740/pexels-photo-1002740.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            "Start Your Morning Right",
            "Freshly brewed specialties crafted with love, every single day."),
        new("https://images.pexels.com/photos/131053/pexels-photo-131053.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            "Artisan Cappuccinos",
            "Perfectly balanced espresso with velvety micro-foam."),
        new("https://images.pexels.com/photos/261434/pexels-photo-261434.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            "Single Origin Beans",
            "Ethically sourced from the world's finest coffee farms."),
    ];

    // Shared category definitions; the first chip ("All") starts selected.
    public static IReadOnlyList<CategoryItem> Categories { get; } =
    [
        new("all", "All", IsSelected: true),
        new("hot", "Hot Drinks"),
        new("cold", "Cold Drinks"),
        new("pastries", "Pastries"),
    ];

    // A little order history so the Orders tab has content on first run; orders placed from the
    // cart are inserted at the top of the shared CartService.Orders state, so the screens share one
    // source of truth.
    public static IImmutableList<OrderRecord> SeedOrders { get; } =
    [
        new("ORD-1042", "Today, 9:14 AM", "Ready for Pickup", 12.25,
            [new("Latte", 1, 5.50), new("Croissant", 2, 3.25)]),
        new("ORD-1041", "Today, 8:02 AM", "Preparing", 10.75,
            [new("Cappuccino", 1, 4.75), new("Iced Matcha", 1, 6.00)]),
        new("ORD-1039", "Yesterday, 3:45 PM", "Completed", 5.50,
            [new("Latte", 1, 5.50)]),
        new("ORD-1037", "Mar 22, 11:30 AM", "Completed", 15.50,
            [new("Cappuccino", 2, 4.75), new("Croissant", 1, 3.25), new("Iced Matcha", 1, 6.00)]),
        new("ORD-1033", "Mar 20, 8:55 AM", "Completed", 9.25,
            [new("Iced Matcha", 1, 6.00), new("Croissant", 1, 3.25)]),
    ];
}
