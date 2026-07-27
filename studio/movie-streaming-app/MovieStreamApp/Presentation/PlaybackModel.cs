namespace MovieStreamApp.Presentation;

/// <summary>
/// Backs <see cref="PlaybackPage"/>. Reached from a movie's "Watch" action via
/// <c>DataViewMap&lt;PlaybackPage, PlaybackModel, Movie&gt;</c>, so Navigation injects the movie being
/// played. A pure projection (no reactive members), so it opts out of the bindable generator.
/// </summary>
[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record PlaybackModel(Movie NowPlaying)
{
    public string VideoThumbUrl => MovieData.TheaterScreen;
    public string CurrentTimeLabel => "38:14";
    public string TotalTimeLabel => "2:18:00";
    public double PlaybackProgress => 0.28;
    public bool IsPlaying => true;
    public string Director => "Maren Okafor";
    public string ReleaseDate => NowPlaying.Year;
    public string AudienceScore => "94%";

    public IReadOnlyList<CastMember> Cast => new[]
    {
        new CastMember("Elias Mercer", "Commander Kane"),
        new CastMember("Yuki Tanaka", "Dr. Solis"),
        new CastMember("Ravi Osei", "Engineer Brax"),
        new CastMember("Clara Voronova", "Mission Control"),
    };

    public IReadOnlyList<Movie> RelatedMovies => MovieData.RelatedTo(NowPlaying);
}
