namespace Commerce.Data;
using Commerce.Data.Models;

public interface IProductEndpoint
{
	ValueTask<ProductData[]> GetAll(CancellationToken ct);

	ValueTask<ReviewData[]> GetReviews(int productId, CancellationToken ct);
}
