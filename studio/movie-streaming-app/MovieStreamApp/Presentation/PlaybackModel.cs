namespace MovieStreamApp.Presentation;

/// <summary>
/// Backs <see cref="PlaybackPage"/>. Reached from a movie's "Watch" action via
/// <c>DataViewMap&lt;PlaybackPage, PlaybackModel, Movie&gt;</c>, so Navigation injects the movie being
/// played. A pure projection (no reactive members), so it opts out of the bindable generator.
/// </summary>
[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record PlaybackModel(Movie NowPlaying)
{
    // The video the player streams, and the still shown as its poster until the first frame decodes.
    // Real playback state (position, duration, playing/paused) is owned by the MediaPlayer in the
    // view, not mirrored here — see PlaybackPage.xaml.cs.
    public string VideoUrl => MovieData.SampleVideo;
    public string VideoThumbUrl => MovieData.TheaterScreen;
    public string Director => MovieData.SampleDirector;
    public string ReleaseDate => NowPlaying.Year;
    public string AudienceScore => MovieData.SampleAudienceScore;

    // Projected from the one shared cast pool (name + role, no avatar) so the playback screen shows the
    // same cast as the movie's detail view instead of a second, contradicting list.
    public IReadOnlyList<CastMember> Cast { get; } =
        MovieData.Cast.Select(c => new CastMember(c.Name, c.Role)).ToList();

    public IReadOnlyList<Movie> RelatedMovies => MovieData.RelatedTo(NowPlaying);
}
