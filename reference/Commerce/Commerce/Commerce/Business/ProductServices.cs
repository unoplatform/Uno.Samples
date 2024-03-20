using CommunityToolkit.Mvvm.Messaging;
using Uno.Extensions.Reactive.Messaging;

namespace Commerce.Business;

public class ProductService : IProductService
{
    private readonly IProductEndpoint _client;
    private readonly IMessenger _messenger;

    private ImmutableHashSet<int> _favorites = ImmutableHashSet<int>.Empty;

    public ProductService(IProductEndpoint client, IMessenger messenger)
    {
        _client = client;
        _messenger = messenger;
    }

    /// <inheritdoc />
    public async ValueTask<IImmutableList<Product>> GetAll(CancellationToken ct)
        => ToProduct(await _client.GetAll(ct));

    /// <inheritdoc />
    public async ValueTask<IImmutableList<Product>> Search(string term, CancellationToken ct)
    {
        var products = (await _client.GetAll(ct)).AsEnumerable();
        if (term is { Length: > 0 })
        {
            products = products.Where(p => p.Name?.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        return ToProduct(products);
    }

    /// <inheritdoc />
    public async ValueTask<IImmutableList<Review>> GetReviews(int productId, CancellationToken ct)
        => (await _client.GetReviews(productId, ct)).Select(data => new Review(data)).ToImmutableList();

    /// <inheritdoc />
    public async ValueTask<IImmutableList<Product>> GetFavorites(CancellationToken ct)
        => (await _client.GetAll(ct))
            .Where(product => _favorites.Contains(product.ProductId))
            .Select(product => new Product(product, isFavorite: true))
            .ToImmutableList();

    /// <inheritdoc />
    public async ValueTask Update(Product product, CancellationToken ct)
    {
        ImmutableInterlocked.Update(
            ref _favorites,
            (favs, prod) => prod.IsFavorite ? favs.Add(prod.ProductId) : favs.Remove(prod.ProductId),
            product);

        _messenger.Send(new EntityMessage<Product>(product.IsFavorite ? EntityChange.Created : EntityChange.Deleted, product));
    }

    private IImmutableList<Product> ToProduct(IEnumerable<ProductData> data)
        => data
            .Select(d => new Product(d, _favorites.Contains(d.ProductId)))
            .ToImmutableList();
}
