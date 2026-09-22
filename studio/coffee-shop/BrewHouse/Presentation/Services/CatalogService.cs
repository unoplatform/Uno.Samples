using BrewHouse.Presentation.MockData;

namespace BrewHouse.Presentation.Services;

// The shop's "backend": everything the app reads rather than owns. Every member is asynchronous and
// takes a CancellationToken, because that is the shape a real endpoint has — which is what lets the
// Models expose IListFeed<T> and the pages render Value / None / Progress / Error through a FeedView
// instead of hand-rolling those branches.
//
// Search is a SERVICE call, not a client-side filter: the criteria go to the service and it returns
// the matching set, the same way a real catalogue API works.
public interface ICatalogService
{
    ValueTask<IImmutableList<HeroBanner>> GetHeroBannersAsync(CancellationToken ct = default);

    ValueTask<IImmutableList<CategoryItem>> GetCategoriesAsync(CancellationToken ct = default);

    ValueTask<IImmutableList<ProductItem>> GetSpecialsAsync(CancellationToken ct = default);

    ValueTask<IImmutableList<ProductItem>> GetFeaturedAsync(CancellationToken ct = default);

    ValueTask<IImmutableList<ProductItem>> SearchProductsAsync(
        string categoryId,
        string? searchText,
        CancellationToken ct = default);

    ValueTask<IImmutableList<OrderRecord>> GetOrderHistoryAsync(CancellationToken ct = default);
}

// The in-memory implementation, standing in for an HTTP endpoint. Swapping this for a real client is
// the only change a live backend needs: the interface, the Models and every page stay as they are.
//
// The small delay is deliberate and load-bearing for the sample, not padding — without any latency a
// feed resolves on the first frame and the FeedView's ProgressTemplate would never be seen. It is
// kept short so the app still feels immediate.
public sealed class CatalogService : ICatalogService
{
    private static readonly TimeSpan Latency = TimeSpan.FromMilliseconds(300);

    public async ValueTask<IImmutableList<HeroBanner>> GetHeroBannersAsync(CancellationToken ct = default)
    {
        await Task.Delay(Latency, ct);
        return CatalogData.HeroBanners.ToImmutableList();
    }

    public async ValueTask<IImmutableList<CategoryItem>> GetCategoriesAsync(CancellationToken ct = default)
    {
        await Task.Delay(Latency, ct);
        return CatalogData.Categories.ToImmutableList();
    }

    public async ValueTask<IImmutableList<ProductItem>> GetSpecialsAsync(CancellationToken ct = default)
    {
        await Task.Delay(Latency, ct);
        return CatalogData.AllProducts.Where(p => p.IsSpecial).ToImmutableList();
    }

    public async ValueTask<IImmutableList<ProductItem>> GetFeaturedAsync(CancellationToken ct = default)
    {
        await Task.Delay(Latency, ct);
        return CatalogData.AllProducts.Where(p => p.IsFeatured).ToImmutableList();
    }

    public async ValueTask<IImmutableList<ProductItem>> SearchProductsAsync(
        string categoryId,
        string? searchText,
        CancellationToken ct = default)
    {
        await Task.Delay(Latency, ct);

        var search = (searchText ?? string.Empty).Trim();
        return CatalogData.AllProducts
            .Where(p => (string.IsNullOrEmpty(categoryId) || categoryId == "all" || p.CategoryId == categoryId)
                        && (search.Length == 0
                            || p.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                            || p.Description.Contains(search, StringComparison.OrdinalIgnoreCase)))
            .ToImmutableList();
    }

    public async ValueTask<IImmutableList<OrderRecord>> GetOrderHistoryAsync(CancellationToken ct = default)
    {
        await Task.Delay(Latency, ct);
        return CatalogData.SeedOrders;
    }
}
