namespace MovieStreamApp.Presentation;

[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record PlaybackModel
{
    public Movie NowPlaying { get; } = new Movie(
        "m-001",
        "The Last Horizon",
        "Sci-Fi",
        "2024",
        "8.4",
        "2h 18m",
        "A lone astronaut discovers a signal from the edge of the universe that could reshape humanity's understanding of existence. A breathtaking journey that blurs the line between science and myth.",
        "https://picsum.photos/seed/sci-fi%20space%20movie%20poster/768/1024",
        true,
        false);

    public string VideoThumbUrl { get; } = "https://picsum.photos/seed/movie%20theater%20cinema%20dark%20screen/1280/720";
    public string CurrentTimeLabel { get; } = "38:14";
    public string TotalTimeLabel { get; } = "2:18:00";
    public double PlaybackProgress { get; } = 0.28;
    public bool IsPlaying { get; } = true;
    public string Director { get; } = "Maren Okafor";
    public string ReleaseDate { get; } = "March 14, 2024";
    public string AudienceScore { get; } = "94%";

    public IReadOnlyList<CastMember> Cast { get; } = new[]
    {
        new CastMember("Elias Mercer", "Commander Kane"),
        new CastMember("Yuki Tanaka", "Dr. Solis"),
        new CastMember("Ravi Osei", "Engineer Brax"),
        new CastMember("Clara Voronova", "Mission Control"),
    };

    public IReadOnlyList<Movie> RelatedMovies { get; } = new[]
    {
        new Movie("m-005", "Solar Drift", "Sci-Fi", "2024", "8.7", "2h 31m",
            "In a dying solar system, a crew races against time to find a new home.",
            "https://picsum.photos/seed/documentary%20nature%20landscape%20film/1280/720", false, true),
        new Movie("m-002", "Crimson Protocol", "Action", "2024", "7.9", "1h 52m",
            "An elite operative uncovers a global conspiracy that puts millions at risk.",
            "https://picsum.photos/seed/cinematic%20movie%20poster%20action/768/1024", false, false),
        new Movie("m-008", "Between Worlds", "Drama", "2024", "8.3", "1h 58m",
            "Two strangers meet at the crossroads of grief and hope.",
            "https://picsum.photos/seed/dark%20thriller%20movie%20poster/768/1024", false, true),
    };
}
