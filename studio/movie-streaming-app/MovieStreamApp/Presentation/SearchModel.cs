namespace MovieStreamApp.Presentation;

/// <summary>
/// Backs <see cref="SearchPage"/>. The search box binds two-way to <see cref="Query"/> and the genre
/// tiles set <see cref="GenreFilter"/>; <see cref="Results"/> is a derived <see cref="IListFeed{Movie}"/>
/// combining both over the shared catalogue, and <see cref="HasResults"/> drives the empty state — no
/// hand-rolled INPC or synced controls (lessons 39, 65).
/// </summary>
public partial record SearchModel
{
    public string SearchPlaceholder => "Search movies, actors, directors...";

    public IReadOnlyList<string> Genres => MovieData.Genres;

    public IReadOnlyList<string> RecentSearches => new[]
    {
        "Christopher Nolan", "Sci-Fi 2024", "Best Documentaries", "Award Winners"
    };

    public IState<string> Query => State.Value(this, () => string.Empty);
    public IState<string> GenreFilter => State.Value(this, () => "All");

    private IFeed<IImmutableList<Movie>> Filtered =>
        Feed.Combine(Query, GenreFilter).Select(t => Search(t.Item1, t.Item2));

    public IListFeed<Movie> Results => Filtered.AsListFeed();
    public IFeed<bool> HasResults => Filtered.Select(list => list.Count > 0);

    // "genre" doesn't match the GenreFilter feed name, so the CommandParameter is used, not the
    // feed's current value (lesson 44).
    public async ValueTask FilterByGenre(string genre, CancellationToken ct = default) =>
        await GenreFilter.SetAsync(genre, ct);

    private static IImmutableList<Movie> Search(string? query, string? genre)
    {
        var q = (query ?? string.Empty).Trim();
        IEnumerable<Movie> source = MovieData.Catalog;

        if (genre is not null and not "All")
        {
            source = source.Where(m => m.Genre == genre);
        }

        if (q.Length > 0)
        {
            source = source.Where(m =>
                m.Title.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                m.Genre.Contains(q, StringComparison.OrdinalIgnoreCase));
        }

        return source.ToImmutableList();
    }
}
