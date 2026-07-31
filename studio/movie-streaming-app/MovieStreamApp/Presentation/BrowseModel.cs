namespace MovieStreamApp.Presentation;

/// <summary>
/// Backs <see cref="BrowsePage"/>. A reactive model: the category chips drive a two-way
/// <see cref="SelectedCategory"/> state, and the Trending / New Arrivals rails are derived
/// <see cref="IListFeed{Movie}"/>s that re-filter when it changes. Watchlist "+" actions write to the
/// shared <see cref="WatchlistService"/> (injected via DI).
/// </summary>
public partial record BrowseModel(WatchlistService Watchlist)
{
    public Movie FeaturedMovie => MovieData.Featured;

    public IReadOnlyList<string> Categories => MovieData.Categories;

    public IState<string> SelectedCategory => State.Value(this, () => "All");

    public IListFeed<Movie> Trending =>
        SelectedCategory.Select(cat => Filter(MovieData.Trending, cat)).AsListFeed();

    public IListFeed<Movie> NewArrivals =>
        SelectedCategory.Select(cat => Filter(MovieData.NewArrivals, cat)).AsListFeed();

    // The parameter is named "category" (not "selectedCategory") so it does NOT match the
    // SelectedCategory feed case-insensitively — otherwise MVUX would inject the feed's current value
    // and discard the chip's CommandParameter (lesson 44).
    public async ValueTask SelectCategory(string category, CancellationToken ct = default) =>
        await SelectedCategory.SetAsync(category, ct);

    public async ValueTask ToggleWatchlist(Movie movie, CancellationToken ct = default) =>
        await Watchlist.ToggleAsync(movie, ct);

    private static IImmutableList<Movie> Filter(IReadOnlyList<Movie> source, string? category) =>
        (category is null or "All" ? source : source.Where(m => m.Genre == category)).ToImmutableList();
}
