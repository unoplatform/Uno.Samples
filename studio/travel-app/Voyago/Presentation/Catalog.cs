namespace Voyago.Presentation;

// Canonical destination catalogue — one source of truth for the seven destinations shown across
// Home (hero + recommended), Search (popular), Favorites (saved) and each category tile's featured
// pick. Sharing the instances guarantees a card and the detail page it opens always carry identical
// data, and lets the category tiles point at a real destination.
//
// NOTE: the destination fields are declared before Categories on purpose — C# initializes static
// fields in textual order, so Categories (which references Paris/Dolomites/…) must come last or the
// featured picks would bind to null.
internal static class Catalog
{
    public static readonly Destination Dolomites = new("d-001", "Dolomites", "Italy",
        "Alpine serenity above the clouds",
        "https://images.pexels.com/photos/28491959/pexels-photo-28491959.jpeg?auto=compress&cs=tinysrgb&w=1200",
        "From EUR 249", 4.9, 2341);

    public static readonly Destination Maldives = new("d-002", "Maldives", "Indian Ocean",
        "Crystal waters, endless horizons",
        "https://images.pexels.com/photos/28843967/pexels-photo-28843967.jpeg?auto=compress&cs=tinysrgb&w=1200",
        "From EUR 899", 4.8, 1875);

    public static readonly Destination Kyoto = new("d-003", "Kyoto", "Japan",
        "Ancient temples, timeless beauty",
        "https://images.pexels.com/photos/16481404/pexels-photo-16481404.jpeg?auto=compress&cs=tinysrgb&w=1200",
        "From EUR 629", 4.7, 3102);

    public static readonly Destination Santorini = new("d-004", "Santorini", "Greece",
        "Cliffs, caldera views, and sunsets",
        "https://images.pexels.com/photos/1010657/pexels-photo-1010657.jpeg?auto=compress&cs=tinysrgb&w=1200",
        "From EUR 399", 4.8, 1562);

    public static readonly Destination Bali = new("d-005", "Bali", "Indonesia",
        "Lush terraces and spiritual calm",
        "https://images.pexels.com/photos/5933066/pexels-photo-5933066.jpeg?auto=compress&cs=tinysrgb&w=1200",
        "From EUR 549", 4.7, 2087);

    public static readonly Destination Paris = new("d-006", "Paris", "France",
        "Romance, cuisine, and art",
        "https://images.pexels.com/photos/532826/pexels-photo-532826.jpeg?auto=compress&cs=tinysrgb&w=1200",
        "From EUR 299", 4.6, 4210);

    public static readonly Destination MachuPicchu = new("d-007", "Machu Picchu", "Peru",
        "Lost city high in the Andes",
        "https://images.pexels.com/photos/2929906/pexels-photo-2929906.jpeg?auto=compress&cs=tinysrgb&w=1200",
        "From EUR 729", 4.9, 987);

    public static readonly Destination Tokyo = new("d-008", "Tokyo", "Japan",
        "Neon nights and serene shrines",
        "https://images.pexels.com/photos/29662430/pexels-photo-29662430.jpeg?auto=compress&cs=tinysrgb&w=1200",
        "From EUR 799", 4.8, 2750);

    // Every destination — used to resolve a trip (which stores only a destination name) back to its
    // full Destination so a trip card can open the detail page. Declared after the statics above.
    public static readonly IReadOnlyList<Destination> All = new[]
    {
        Dolomites, Maldives, Kyoto, Santorini, Bali, Paris, MachuPicchu, Tokyo,
    };

    public static Destination? ByName(string name) => All.FirstOrDefault(d => d.Name == name);

    // Explore/Browse-by-style tiles. Each carries a representative destination (Featured) so tapping
    // the tile opens a real detail page — Home and Search share this one definition.
    public static readonly IReadOnlyList<ExploreCategory> Categories = new[]
    {
        new ExploreCategory("ec-01", "City Breaks",
            "https://images.pexels.com/photos/20847307/pexels-photo-20847307.jpeg?auto=compress&cs=tinysrgb&w=1200",
            "Urban adventures in iconic metropolises", Paris),
        new ExploreCategory("ec-02", "Nature Escapes",
            "https://images.pexels.com/photos/417074/pexels-photo-417074.jpeg?auto=compress&cs=tinysrgb&w=1200",
            "Reconnect with the wild", Dolomites),
        new ExploreCategory("ec-03", "Romantic Getaways",
            "https://images.pexels.com/photos/3546189/pexels-photo-3546189.jpeg?auto=compress&cs=tinysrgb&w=1200",
            "Unforgettable moments for two", Santorini),
        new ExploreCategory("ec-04", "Cultural Trips",
            "https://images.pexels.com/photos/15890613/pexels-photo-15890613.jpeg?auto=compress&cs=tinysrgb&w=1200",
            "History, heritage, and discovery", Kyoto),
    };
}
