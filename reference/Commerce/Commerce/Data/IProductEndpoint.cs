namespace Commerce.Data;

public interface IProductEndpoint
{
	ValueTask<ProductData[]> GetAll(CancellationToken ct);

	ValueTask<ReviewData[]> GetReviews(int productId, CancellationToken ct);
}
