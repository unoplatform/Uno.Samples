namespace MovieStreamApp.Presentation;

[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record MovieDetailModel
{
    // ── Hero ──────────────────────────────────────────────────────────────────
    public string HeroImageUrl { get; } = "https://picsum.photos/seed/epic%20action%20movie%20cinematic%20poster%20widescreen/1280/720";
    public string PosterImageUrl { get; } = "https://picsum.photos/seed/cinematic%20movie%20poster%20action/768/1024";

    // ── Movie metadata ────────────────────────────────────────────────────────
    public string Title { get; } = "Crimson Horizon";
    public string Tagline { get; } = "The line between hero and legend is drawn in blood.";
    public string Genre { get; } = "Action · Thriller";
    public string Year { get; } = "2024";
    public string Rating { get; } = "8.4";
    public string Duration { get; } = "2h 18m";
    public string AgeRating { get; } = "R";
    public string Director { get; } = "Elena Vasquez";
    public string Synopsis { get; } = "When a covert operative discovers a conspiracy reaching the highest levels of government, she must go off-grid and race across three continents to expose the truth before it destroys everything she's sworn to protect. A relentless, visually stunning thriller that redefines the genre.";
    public bool IsInWatchlist { get; } = false;
    public string AudienceScore { get; } = "94%";
    public string CriticsScore { get; } = "87%";
    public int ReviewCount { get; } = 2841;

    // ── Stars ─────────────────────────────────────────────────────────────────
    public string StarDisplay { get; } = "\uE735\uE735\uE735\uE735\uE734";

    // ── Cast ──────────────────────────────────────────────────────────────────
    public IReadOnlyList<CastMemberDetail> Cast { get; } = new[]
    {
        new CastMemberDetail("Mara Solano", "Sofia Reyes", "https://picsum.photos/seed/actor%20actress%20headshot%20professional%20film/1024/1024"),
        new CastMemberDetail("James Carver", "Director Cole", "https://picsum.photos/seed/man%20portrait%20professional%20headshot/1024/1024"),
        new CastMemberDetail("Priya Nath", "Dr. Ananya", "https://picsum.photos/seed/woman%20portrait%20casual%20smiling/1024/1024"),
        new CastMemberDetail("Leo Brandt", "Viktor Cross", "https://picsum.photos/seed/movie%20star%20portrait%20celebrity%20closeup/1024/1024"),
        new CastMemberDetail("Dana Park", "The Broker", "https://picsum.photos/seed/film%20director%20crew%20behind%20the%20scenes/1024/1024"),
    };

    // ── Reviews ───────────────────────────────────────────────────────────────
    public IReadOnlyList<Review> Reviews { get; } = new[]
    {
        new Review("r-001", "CineFreak99", "https://picsum.photos/seed/person%20portrait%20smiling%20profile%20photo/1024/1024", 5, "One of the best action films of the decade. The practical stunts are jaw-dropping and the script never loses momentum. Elena Vasquez cements her place among the greats.", "2d ago", 312),
        new Review("r-002", "FilmNerd_Jules", "https://picsum.photos/seed/young%20person%20smiling%20casual%20portrait/1024/1024", 4, "Riveting from the first minute. Mara Solano is a revelation — magnetic and ferocious. The third-act twist genuinely shocked me. Minor pacing issues in the second act but easily forgiven.", "5d ago", 189),
        new Review("r-003", "MidnightCritic", "https://picsum.photos/seed/friends%20group%20watching%20movie%20together/1024/1024", 4, "Gorgeous cinematography, tight editing, and a score that lodges itself in your brain. A must-watch on the biggest screen you can find.", "1w ago", 97),
    };

    // ── Related movies ────────────────────────────────────────────────────────
    public IReadOnlyList<Movie> RelatedMovies { get; } = new[]
    {
        new Movie("rm-01", "Shadow Protocol", "Thriller", "2023", "7.9", "2h 05m", "A spy uncovers a double agent within her own unit.", "https://picsum.photos/seed/dark%20thriller%20movie%20poster/768/1024", false, false),
        new Movie("rm-02", "The Last Signal", "Sci-Fi", "2024", "8.1", "2h 12m", "A distress call from the edge of the known universe changes everything.", "https://picsum.photos/seed/sci-fi%20space%20movie%20poster/768/1024", false, true),
        new Movie("rm-03", "Raven's Code", "Action", "2023", "7.6", "1h 58m", "A hacker becomes the world's last line of defence.", "https://picsum.photos/seed/superhero%20action%20blockbuster%20movie/768/1024", false, false),
        new Movie("rm-04", "Null Point", "Drama", "2024", "8.6", "2h 30m", "Two agents on opposite sides of a conflict realise they're fighting for the same cause.", "https://picsum.photos/seed/drama%20romance%20movie%20poster/768/1024", false, true),
    };
}

public partial record CastMemberDetail(
    string Name,
    string Role,
    string ImageUrl);
