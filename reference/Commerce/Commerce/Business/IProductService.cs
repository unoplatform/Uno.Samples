namespace Commerce.Business;

public interface IProductService
{
	ValueTask<IImmutableList<Product>> GetAll(CancellationToken ct);

	ValueTask<IImmutableList<Product>> Search(string term, CancellationToken ct);

	ValueTask<IImmutableList<Review>> GetReviews(int productId, CancellationToken ct);

	ValueTask<IImmutableList<Product>> GetFavorites(CancellationToken ct);

	ValueTask Update(Product product, CancellationToken ct);
}
