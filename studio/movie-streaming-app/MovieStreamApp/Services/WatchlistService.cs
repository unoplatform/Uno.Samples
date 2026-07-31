using MovieStreamApp.Presentation;

namespace MovieStreamApp.Services;

/// <summary>
/// The app-lifetime "My List" store. Registered as a DI singleton and injected into every page-Model
/// that reads or mutates the watchlist, so all of them share the SAME <see cref="IListState{Movie}"/>:
/// toggling a movie on the detail page updates the Browse "+" state and the Profile count live, with
/// no messaging or manual change fan-out (MVUX shared-state pattern).
/// </summary>
public sealed class WatchlistService
{
    // Once-initialized in the ctor (NOT an expression-bodied getter, which would rebuild a fresh
    // state on every access and break sharing). The singleton itself is the reactive owner.
    public IListState<Movie> Movies { get; }

    public WatchlistService()
    {
        Movies = ListState.Value(this, () => MovieData.InitialWatchlist);
    }

    /// <summary>Adds the movie if it isn't in the list, otherwise removes it (matched by key).</summary>
    public async ValueTask ToggleAsync(Movie movie, CancellationToken ct = default)
    {
        var current = await Movies.Value(ct);
        if (current is not null && current.Any(m => m.Id == movie.Id))
        {
            await Movies.RemoveAllAsync(m => m.Id == movie.Id, ct);
        }
        else
        {
            await Movies.InsertAsync(movie, ct);
        }
    }
}
