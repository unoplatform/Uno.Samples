namespace MovieStreamApp.Presentation;

/// <summary>
/// Backs the movie detail views (a full-screen <see cref="MovieDetailPage"/> on phone/tablet and the
/// <see cref="MovieDetailDialog"/> modal on desktop — two DataViewMaps, one model). Navigation injects
/// the tapped <see cref="Movie"/> as the first ctor parameter; the shared <see cref="WatchlistService"/>
/// resolves from DI (lesson 39). Reactive (it derives <see cref="IsInWatchlist"/> from the shared store),
/// so it is NOT [ReactiveBindable(false)].
/// </summary>
public partial record MovieDetailModel(Movie Movie, WatchlistService Watchlist)
{
    public string HeroImageUrl => MovieData.HeroEpic;
    public string PosterImageUrl => Movie.ImageUrl;
    public string Tagline => MovieData.SampleTagline;
    public string AgeRating => MovieData.SampleAgeRating;
    public string Director => MovieData.SampleDirector;
    public string AudienceScore => MovieData.SampleAudienceScore;
    public string CriticsScore => MovieData.SampleCriticsScore;
    public int ReviewCount => MovieData.SampleReviewCount;

    // Five booleans (rating/2, rounded) — the view renders a filled or outline star per position.
    public IReadOnlyList<bool> Stars => MovieData.Stars(Movie.Rating);

    public IReadOnlyList<CastMemberDetail> Cast => MovieData.Cast;
    public IReadOnlyList<Review> Reviews => MovieData.Reviews;
    public IReadOnlyList<Movie> RelatedMovies => MovieData.RelatedTo(Movie);

    // Reflects the shared store: opens showing the real "in list" state and flips live when toggled.
    public IFeed<bool> IsInWatchlist =>
        Watchlist.Movies.AsFeed().Select(list => list.Any(m => m.Id == Movie.Id));

    public async ValueTask ToggleWatchlist(CancellationToken ct = default) =>
        await Watchlist.ToggleAsync(Movie, ct);
}
