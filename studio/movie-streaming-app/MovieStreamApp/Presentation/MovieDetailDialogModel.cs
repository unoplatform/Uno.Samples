namespace MovieStreamApp.Presentation;

/// <summary>
/// Backs the desktop <see cref="MovieDetailDialog"/> modal. It mirrors <see cref="MovieDetailModel"/>
/// (the phone/tablet page) member-for-member and both host the shared <c>MovieDetailContent</c>.
///
/// It is a SEPARATE view-model type on purpose: a reactive (bindable-generated) model canNOT be shared
/// across two DataViewMaps — the reactive view-model mapping is keyed by view-model type, so registering
/// the same reactive type for both the page and the dialog silently breaks its factory and the detail
/// route falls back to the default tab. Two distinct types (one per view) keep both working.
/// </summary>
public partial record MovieDetailDialogModel(Movie Movie, WatchlistService Watchlist)
{
    public string HeroImageUrl => MovieData.HeroEpic;
    public string PosterImageUrl => Movie.ImageUrl;
    public string Tagline => "Every choice has a price. Hers just came due.";
    public string AgeRating => "16+";
    public string Director => "Elena Vasquez";
    public string AudienceScore => "94%";
    public string CriticsScore => "87%";
    public int ReviewCount => 2841;

    public IReadOnlyList<bool> Stars => MovieData.Stars(Movie.Rating);

    public IReadOnlyList<CastMemberDetail> Cast => MovieData.Cast;
    public IReadOnlyList<Review> Reviews => MovieData.Reviews;
    public IReadOnlyList<Movie> RelatedMovies => MovieData.RelatedTo(Movie);

    public IFeed<bool> IsInWatchlist =>
        Watchlist.Movies.AsFeed().Select(list => list.Any(m => m.Id == Movie.Id));

    public async ValueTask ToggleWatchlist(CancellationToken ct = default) =>
        await Watchlist.ToggleAsync(Movie, ct);
}
