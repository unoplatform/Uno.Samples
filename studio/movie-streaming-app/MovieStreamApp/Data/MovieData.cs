using MovieStreamApp.Presentation;

namespace MovieStreamApp.Data;

/// <summary>
/// The single seeded movie catalogue for the whole app. Every page reads its movies from here
/// (keyed by <see cref="Movie.Id"/>) so a card can resolve the full entity to open its detail,
/// and the watchlist / search / category filters all operate over one shared source.
/// Images are specific, subject-matched remote photos (never bundled Content assets, never a
/// random placeholder seed).
/// </summary>
public static class MovieData
{
    // ── Poster / backdrop / portrait image slots (cinematic mood photos, not real posters) ──
    private const string PosterSciFi = "https://images.pexels.com/photos/3180831/pexels-photo-3180831.jpeg?auto=compress&cs=tinysrgb&w=800";
    private const string PosterAction = "https://images.pexels.com/photos/12356447/pexels-photo-12356447.jpeg?auto=compress&cs=tinysrgb&w=800";
    private const string PosterDrama = "https://images.pexels.com/photos/1024963/pexels-photo-1024963.jpeg?auto=compress&cs=tinysrgb&w=800";
    private const string PosterHorror = "https://images.pexels.com/photos/7016272/pexels-photo-7016272.jpeg?auto=compress&cs=tinysrgb&w=800";
    private const string PosterComedy = "https://images.pexels.com/photos/3625632/pexels-photo-3625632.jpeg?auto=compress&cs=tinysrgb&w=800";
    private const string PosterSuperhero = "https://images.pexels.com/photos/12695408/pexels-photo-12695408.jpeg?auto=compress&cs=tinysrgb&w=800";
    private const string PosterThriller = "https://images.pexels.com/photos/14220329/pexels-photo-14220329.jpeg?auto=compress&cs=tinysrgb&w=800";
    private const string BackdropDocumentary = "https://images.pexels.com/photos/9166681/pexels-photo-9166681.jpeg?auto=compress&cs=tinysrgb&w=1200";

    public const string HeroEpic = "https://images.pexels.com/photos/17845888/pexels-photo-17845888.jpeg?auto=compress&cs=tinysrgb&w=1200";
    public const string TheaterScreen = "https://images.pexels.com/photos/7991149/pexels-photo-7991149.jpeg?auto=compress&cs=tinysrgb&w=1200";
    // Small demo clip the Playback screen streams via MediaPlayerElement, bundled as an ms-appx asset
    // so it plays offline on every head with no network/CORS/User-Agent concerns. A tiny (~135 KB)
    // 240p baseline-H.264 encode of Blender's CC-BY "Big Buck Bunny" — low-profile so it decodes even
    // on the iOS Simulator (which fails high-profile 720p streams) and small enough not to bloat the repo.
    public const string SampleVideo = "ms-appx:///Assets/Media/big-buck-bunny-clip.mp4";
    public const string OnboardingHero = "https://images.pexels.com/photos/19281432/pexels-photo-19281432.jpeg?auto=compress&cs=tinysrgb&w=1200";
    public const string FestivalBanner = "https://images.pexels.com/photos/2504971/pexels-photo-2504971.jpeg?auto=compress&cs=tinysrgb&w=1200";

    // Portraits (cast / friends / reviewers / user)
    public const string PortraitWoman = "https://images.pexels.com/photos/11749490/pexels-photo-11749490.jpeg?auto=compress&cs=tinysrgb&w=800";
    public const string PortraitMan = "https://images.pexels.com/photos/12311572/pexels-photo-12311572.jpeg?auto=compress&cs=tinysrgb&w=800";
    public const string PortraitYoung = "https://images.pexels.com/photos/6338370/pexels-photo-6338370.jpeg?auto=compress&cs=tinysrgb&w=800";
    public const string PortraitProfile = "https://images.pexels.com/photos/11583453/pexels-photo-11583453.jpeg?auto=compress&cs=tinysrgb&w=800";
    public const string PortraitFriends = "https://images.pexels.com/photos/8555880/pexels-photo-8555880.jpeg?auto=compress&cs=tinysrgb&w=1200";
    public const string PortraitActress = "https://images.pexels.com/photos/6899791/pexels-photo-6899791.jpeg?auto=compress&cs=tinysrgb&w=800";
    public const string PortraitStar = "https://images.pexels.com/photos/5542500/pexels-photo-5542500.jpeg?auto=compress&cs=tinysrgb&w=800";
    public const string PortraitDirector = "https://images.pexels.com/photos/7513459/pexels-photo-7513459.jpeg?auto=compress&cs=tinysrgb&w=1200";
    public const string UserAvatar = "https://images.pexels.com/photos/6497851/pexels-photo-6497851.jpeg?auto=compress&cs=tinysrgb&w=800";

    // ── The catalogue ───────────────────────────────────────────────────────────────────────
    public static IReadOnlyList<Movie> Catalog { get; } = new[]
    {
        new Movie("m-001", "The Last Horizon", "Sci-Fi", "2024", "8.4", "2h 18m",
            "A lone astronaut discovers a signal from the edge of the universe that could reshape humanity's understanding of existence. A breathtaking journey into the unknown.",
            PosterSciFi, IsFeatured: true, IsNew: false),
        new Movie("m-002", "Crimson Protocol", "Action", "2024", "7.9", "1h 52m",
            "An elite operative uncovers a global conspiracy that puts millions at risk.",
            PosterAction, false, true),
        new Movie("m-003", "Shattered Glass", "Drama", "2024", "8.1", "2h 05m",
            "A celebrated artist's life unravels when a forgotten past resurfaces.",
            PosterDrama, false, false),
        new Movie("m-004", "Void Walker", "Horror", "2023", "7.5", "1h 44m",
            "Something in the darkness has been watching since the beginning.",
            PosterHorror, false, false),
        new Movie("m-005", "Solar Drift", "Sci-Fi", "2024", "8.7", "2h 31m",
            "In a dying solar system, a crew races against time to find a new home for mankind.",
            PosterSciFi, false, true),
        new Movie("m-006", "The Wildest Show", "Comedy", "2024", "7.3", "1h 38m",
            "A chaotic talent-show audition weekend spirals into the most unforgettable 48 hours.",
            PosterComedy, false, false),
        new Movie("m-007", "Iron Veil", "Action", "2024", "7.8", "2h 10m",
            "A disbanded special forces unit is called back for one final impossible mission.",
            PosterSuperhero, false, true),
        new Movie("m-008", "Between Worlds", "Drama", "2024", "8.3", "1h 58m",
            "Two strangers meet at the crossroads of grief and hope in a timeless love story.",
            PosterDrama, false, true),
        new Movie("m-009", "Earth Reborn", "Documentary", "2024", "9.0", "1h 30m",
            "An astonishing look at how life regenerates in the most hostile environments.",
            BackdropDocumentary, false, true),
        new Movie("m-010", "Shadow Protocol", "Thriller", "2023", "7.9", "2h 05m",
            "A spy uncovers a double agent within her own unit.",
            PosterThriller, false, false),
        new Movie("m-011", "The Last Signal", "Sci-Fi", "2024", "8.1", "2h 12m",
            "A distress call from the edge of the known universe changes everything.",
            PosterSciFi, false, true),
        new Movie("m-012", "Raven's Code", "Action", "2023", "7.6", "1h 58m",
            "A hacker becomes the world's last line of defence.",
            PosterSuperhero, false, false),
        new Movie("m-013", "Null Point", "Drama", "2024", "8.6", "2h 30m",
            "Two agents on opposite sides of a conflict realise they're fighting for the same cause.",
            PosterDrama, false, true),
    };

    public static Movie Featured { get; } = ById("m-001");

    // Genre facets used by the Browse category chips and Search genre tiles. "All" is the reset.
    public static IReadOnlyList<string> Categories { get; } =
        new[] { "All", "Action", "Drama", "Sci-Fi", "Horror", "Comedy", "Documentary", "Thriller" };

    public static IReadOnlyList<string> Genres { get; } =
        new[] { "Action", "Drama", "Sci-Fi", "Horror", "Comedy", "Documentary", "Thriller" };

    public static IReadOnlyList<Movie> Trending { get; } = Ids("m-002", "m-005", "m-004", "m-003", "m-006", "m-007");
    public static IReadOnlyList<Movie> NewArrivals { get; } = Ids("m-007", "m-008", "m-009", "m-011", "m-013");

    // Seeded so the watchlist / Profile counts read non-empty on first launch.
    public static ImmutableList<Movie> InitialWatchlist { get; } = Ids("m-001", "m-005", "m-008").ToImmutableList();

    // ── Shared cast / review pools (a mockup has no per-title cast) ──────────────────────────
    public static IReadOnlyList<CastMemberDetail> Cast { get; } = new[]
    {
        new CastMemberDetail("Mara Solano", "Lead", PortraitActress),
        new CastMemberDetail("James Carver", "Supporting", PortraitMan),
        new CastMemberDetail("Priya Nath", "Supporting", PortraitWoman),
        new CastMemberDetail("Leo Brandt", "Antagonist", PortraitStar),
        new CastMemberDetail("Dana Park", "Director", PortraitDirector),
    };

    public static IReadOnlyList<Review> Reviews { get; } = new[]
    {
        new Review("r-001", "CineFreak99", PortraitProfile, 5, "One of the best films of the decade. The craft is jaw-dropping and the script never loses momentum.", "2d ago", 312),
        new Review("r-002", "FilmNerd_Jules", PortraitYoung, 4, "Riveting from the first minute. The third-act twist genuinely shocked me. Minor pacing issues, easily forgiven.", "5d ago", 189),
        new Review("r-003", "MidnightCritic", PortraitFriends, 4, "Gorgeous cinematography, tight editing, and a score that lodges itself in your brain.", "1w ago", 97),
    };

    // ── Lookups ─────────────────────────────────────────────────────────────────────────────
    public static Movie ById(string id) =>
        Catalog.FirstOrDefault(m => m.Id == id) ?? Catalog[0];

    // Resolve a movie by its title (Social feed / friend activity stores only a title). Falls
    // back to the featured movie so a card's Navigation.Data is never null.
    public static Movie ByTitle(string title) =>
        Catalog.FirstOrDefault(m => string.Equals(m.Title, title, StringComparison.OrdinalIgnoreCase)) ?? Featured;

    // "More like this" — other titles sharing the genre (or a broad fallback if too few).
    public static IReadOnlyList<Movie> RelatedTo(Movie movie)
    {
        var same = Catalog.Where(m => m.Id != movie.Id && m.Genre == movie.Genre).ToList();
        if (same.Count < 3)
        {
            same.AddRange(Catalog.Where(m => m.Id != movie.Id && m.Genre != movie.Genre));
        }
        return same.Take(6).ToList();
    }

    public static IReadOnlyList<Movie> Ids(params string[] ids) =>
        ids.Select(ById).ToList();

    // A 0-10 rating string -> five booleans (rating/2, rounded) for a discrete star row.
    public static IReadOnlyList<bool> Stars(string rating)
    {
        double.TryParse(rating, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out var r);
        var filled = (int)Math.Round(r / 2, MidpointRounding.AwayFromZero);
        return Enumerable.Range(1, 5).Select(i => i <= filled).ToList();
    }
}
