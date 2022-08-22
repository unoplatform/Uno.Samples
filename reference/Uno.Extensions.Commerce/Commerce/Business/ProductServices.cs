using CommunityToolkit.Mvvm.Messaging;
using Uno.Extensions.Reactive.Messaging;

namespace Commerce.Business;

public class ProductService : IProductService
{
	private readonly IProductEndpoint _productEndpoint;
    private readonly IReviewsEndpoint _reviewsEndpoint;
    private readonly IMessenger _messenger;

	private ImmutableHashSet<int> _favorites = ImmutableHashSet<int>.Empty;

	public ProductService(
		IProductEndpoint productEndpoint,
        IReviewsEndpoint reviewsEndpoint,
        IMessenger messenger)
	{
		_productEndpoint = productEndpoint;
		_reviewsEndpoint = reviewsEndpoint;
		_messenger = messenger;
	}

	/// <inheritdoc />
	public async ValueTask<IImmutableList<Product>> GetAll(CancellationToken ct)
		=> ToProduct((await _productEndpoint.ProductsAsync(ct,0,100)).Products??Array.Empty<ProductData>());

	/// <inheritdoc />
	public async ValueTask<IImmutableList<Product>> Search(string term, CancellationToken ct)
	{
		var products = (await GetAll(ct)).AsEnumerable();
		if (term is { Length: > 0 })
		{
			products = products.Where(p => p.Name?.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0);
		}

		return products.ToImmutableList();
	}

	/// <inheritdoc />
	public async ValueTask<IImmutableList<Review>> GetReviews(int productId, CancellationToken ct)
		=> (await _reviewsEndpoint.GetReviews(productId, ct)).Select(data => new Review(data)).ToImmutableList();

	/// <inheritdoc />
	public async ValueTask<IImmutableList<Product>> GetFavorites(CancellationToken ct)
		=> (await GetAll(ct))
			.Where(product => _favorites.Contains(product.ProductId))
			.Select(product => product with { IsFavorite = true })
			.ToImmutableList();

	/// <inheritdoc />
	public ValueTask Update(Product product, CancellationToken ct)
	{
		ImmutableInterlocked.Update(
			ref _favorites,
			(favs, prod) => prod.IsFavorite ? favs.Add(prod.ProductId) : favs.Remove(prod.ProductId),
			product);

		_messenger.Send(new EntityMessage<Product>(product.IsFavorite ? EntityChange.Created : EntityChange.Deleted, product));

		return ValueTask.CompletedTask;
	}

	private IImmutableList<Product> ToProduct(IEnumerable<ProductData> data)
		=> data
			.Select(d => new Product(d, _favorites.Contains(d.ProductId)))
			.ToImmutableList();
}
