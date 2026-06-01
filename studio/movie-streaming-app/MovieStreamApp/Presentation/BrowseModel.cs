namespace MovieStreamApp.Presentation;

[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record BrowseModel
{
    public Movie FeaturedMovie { get; } = new Movie(
        "m-001",
        "The Last Horizon",
        "Sci-Fi",
        "2024",
        "8.4",
        "2h 18m",
        "A lone astronaut discovers a signal from the edge of the universe that could reshape humanity's understanding of existence. A breathtaking journey into the unknown.",
        "https://picsum.photos/seed/sci-fi%20space%20movie%20poster/768/1024",
        true,
        false);

    public IReadOnlyList<string> Categories { get; } = new[]
    {
        "All", "Action", "Drama", "Sci-Fi", "Horror", "Comedy", "Documentary"
    };

    public string SelectedCategory { get; } = "All";

    public IReadOnlyList<Movie> TrendingNow { get; } = new[]
    {
        new Movie("m-002", "Crimson Protocol", "Action", "2024", "7.9", "1h 52m",
            "An elite operative uncovers a global conspiracy that puts millions at risk.",
            "https://picsum.photos/seed/cinematic%20movie%20poster%20action/768/1024", false, true),
        new Movie("m-003", "Shattered Glass", "Drama", "2024", "8.1", "2h 05m",
            "A celebrated artist's life unravels when a forgotten past resurfaces.",
            "https://picsum.photos/seed/drama%20romance%20movie%20poster/768/1024", false, false),
        new Movie("m-004", "Void Walker", "Horror", "2023", "7.5", "1h 44m",
            "Something in the darkness has been watching since the beginning.",
            "https://picsum.photos/seed/horror%20mystery%20movie%20poster%20dark/768/1024", false, false),
        new Movie("m-005", "Solar Drift", "Sci-Fi", "2024", "8.7", "2h 31m",
            "In a dying solar system, a crew races against time to find a new home for mankind.",
            "https://picsum.photos/seed/documentary%20nature%20landscape%20film/1280/720", false, true),
        new Movie("m-006", "The Wildest Show", "Comedy", "2024", "7.3", "1h 38m",
            "A chaotic talent-show audition weekend spirals into the most unforgettable 48 hours.",
            "https://picsum.photos/seed/comedy%20animated%20movie%20poster%20colorful/768/1024", false, false),
    };

    public IReadOnlyList<Movie> NewArrivals { get; } = new[]
    {
        new Movie("m-007", "Iron Veil", "Action", "2024", "7.8", "2h 10m",
            "A disbanded special forces unit is called back for one final impossible mission.",
            "https://picsum.photos/seed/superhero%20action%20blockbuster%20movie/768/1024", false, true),
        new Movie("m-008", "Between Worlds", "Drama", "2024", "8.3", "1h 58m",
            "Two strangers meet at the crossroads of grief and hope in a timeless love story.",
            "https://picsum.photos/seed/dark%20thriller%20movie%20poster/768/1024", false, true),
        new Movie("m-009", "Earth Reborn", "Documentary", "2024", "9.0", "1h 30m",
            "An astonishing look at how life regenerates in the most hostile environments.",
            "https://picsum.photos/seed/documentary%20nature%20landscape%20film/1280/720", false, true),
    };
}
