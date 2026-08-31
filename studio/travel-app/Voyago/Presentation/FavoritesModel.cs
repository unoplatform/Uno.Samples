using Voyago.Presentation.Services;

namespace Voyago.Presentation;

// The traveller's saved destinations, read from IDiscoveryService and rendered through a FeedView —
// so "nothing saved yet" is a designed state rather than a blank page.
public partial record FavoritesModel(IDiscoveryService Discovery)
{
    private IListFeed<Destination>? _saved;
    public IListFeed<Destination> SavedDestinations =>
        _saved ??= ListFeed.Async(Discovery.GetSavedDestinationsAsync);

    // The header count. SelectData, not Select: an empty list feed emits None and Select skips None,
    // so a plain projection would render the count BLANK on an empty list instead of "0".
    public IFeed<int> TotalFavorites =>
        SavedDestinations
            .AsFeed()
            .SelectData<IImmutableList<Destination>, int>(items => items.SomeOrDefault()?.Count ?? 0);
}
