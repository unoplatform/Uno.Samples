namespace Voyago.Presentation;

[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record FavoritesModel
{
    public int TotalFavorites { get; } = 7;

    public IReadOnlyList<Destination> SavedDestinations { get; } = new[]
    {
        Catalog.Santorini, Catalog.Dolomites, Catalog.Maldives, Catalog.MachuPicchu,
        Catalog.Kyoto, Catalog.Bali, Catalog.Paris,
    };
}
