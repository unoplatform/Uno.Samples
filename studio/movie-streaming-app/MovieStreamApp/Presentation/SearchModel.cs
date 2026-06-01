namespace MovieStreamApp.Presentation;

[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record SearchModel
{
    public string SearchQuery { get; } = "";
    public string SearchPlaceholder { get; } = "Search movies, actors, directors...";

    public IReadOnlyList<GenreCategory> Genres { get; } = new[]
    {
        new GenreCategory("Action", "\uE945", "#CC2233"),
        new GenreCategory("Drama", "\uEB51", "#7A3D80"),
        new GenreCategory("Sci-Fi", "\uE714", "#1A5DA6"),
        new GenreCategory("Horror", "\uE7BA", "#333333"),
        new GenreCategory("Comedy", "\uE76E", "#D67A00"),
        new GenreCategory("Documentary", "\uE8B7", "#1A7A4A"),
        new GenreCategory("Romance", "\uEB52", "#C23070"),
        new GenreCategory("Thriller", "\uE8B3", "#4A3070"),
    };

    public IReadOnlyList<Movie> SearchResults { get; } = new[]
    {
        new Movie("m-002", "Crimson Protocol", "Action", "2024", "7.9", "1h 52m",
            "An elite operative uncovers a global conspiracy that puts millions at risk.",
            "https://picsum.photos/seed/cinematic%20movie%20poster%20action/768/1024", false, true),
        new Movie("m-001", "The Last Horizon", "Sci-Fi", "2024", "8.4", "2h 18m",
            "A lone astronaut discovers a signal from the edge of the universe.",
            "https://picsum.photos/seed/sci-fi%20space%20movie%20poster/768/1024", true, false),
        new Movie("m-003", "Shattered Glass", "Drama", "2024", "8.1", "2h 05m",
            "A celebrated artist's life unravels when a forgotten past resurfaces.",
            "https://picsum.photos/seed/drama%20romance%20movie%20poster/768/1024", false, false),
        new Movie("m-007", "Iron Veil", "Action", "2024", "7.8", "2h 10m",
            "A disbanded special forces unit is called back for one final impossible mission.",
            "https://picsum.photos/seed/superhero%20action%20blockbuster%20movie/768/1024", false, true),
        new Movie("m-004", "Void Walker", "Horror", "2023", "7.5", "1h 44m",
            "Something in the darkness has been watching since the beginning.",
            "https://picsum.photos/seed/horror%20mystery%20movie%20poster%20dark/768/1024", false, false),
        new Movie("m-009", "Earth Reborn", "Documentary", "2024", "9.0", "1h 30m",
            "An astonishing look at how life regenerates in the most hostile environments.",
            "https://picsum.photos/seed/documentary%20nature%20landscape%20film/1280/720", false, true),
    };

    public IReadOnlyList<string> RecentSearches { get; } = new[]
    {
        "Christopher Nolan", "Sci-Fi 2024", "Best Documentaries", "Award Winners"
    };
}

public partial record GenreCategory(
    string Name,
    string IconGlyph,
    string AccentHex);
