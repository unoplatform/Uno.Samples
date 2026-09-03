namespace Voyago.Presentation.MockData;

/// <summary>
/// Design-time DataContext for <see cref="FavoritesPage"/> in Hot Design / Studio, built to the recipe
/// documented on <see cref="HomePageMockData"/>. The saved grid is a list feed because a FeedView
/// renders it; the header count is a materialized int, since the live Model derives it through a
/// projection a design surface has no context to pump.
/// </summary>
[ReactiveBindable]
public partial record FavoritesPageMockData
{
    /// <summary>A well-used account: seven destinations saved.</summary>
    public static FavoritesPageMockDataViewModel Data => new();

    // Nothing saved — the grid's NoneTemplate, and the header count reading 0 rather than blank
    // (which is what the Model's SelectData projection is for).
    public static FavoritesPageMockDataViewModel Empty =>
        FavoritesPageMockDataViewModel.ForModel(new()
        {
            SavedDestinations = ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<Destination>>(
                ImmutableList<Destination>.Empty)),
            TotalFavorites = 0,
        });

    public IListFeed<Destination> SavedDestinations { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<Destination>>(
            ImmutableList.Create(
                Catalog.Santorini, Catalog.Dolomites, Catalog.Maldives, Catalog.MachuPicchu,
                Catalog.Kyoto, Catalog.Bali, Catalog.Paris)));

    public int TotalFavorites { get; init; } = 7;
}

// Reaches the generated ViewModel's protected model-taking constructor, so Empty can wrap a
// customized model. See HomePageMockDataViewModel for the full explanation.
public partial class FavoritesPageMockDataViewModel
{
    internal static FavoritesPageMockDataViewModel ForModel(FavoritesPageMockData model) => new(model);
}
