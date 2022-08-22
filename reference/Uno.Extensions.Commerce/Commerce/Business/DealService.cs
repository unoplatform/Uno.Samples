namespace Commerce.Business;

public class DealService : IDealService
{
	private readonly IProductEndpoint _productEndpoint;

	public DealService(IProductEndpoint productEndpoint)
	{
		_productEndpoint=productEndpoint; 
	}

	public async ValueTask<IImmutableList<Product>> GetPaginated(CancellationToken ct, int from, int size)
	{
		var deals = await _productEndpoint.ProductsAsync(ct, from, size);

		var paginated = deals
			.Products?
			.Select(data => new Product(data, isFavorite: /* TODO */ false))
            .ToImmutableList();

		return paginated??ImmutableList<Product>.Empty;
	}
}
