namespace Commerce.Business;
using Commerce.Business.Models;
using Commerce.Data.Models;
using Commerce.Data;

public class DealService : IDealService
{
	private readonly IStorage _dataService;
	private readonly ISerializer _serializer;

	public DealService(IStorage dataService, ISerializer serializer)
	{
		_dataService = dataService;
		_serializer = serializer;
	}

	private async ValueTask<IEnumerable<Product>> GetDeals()
	{
		var products = await _dataService.ReadPackageFileAsync<ProductData[]>(_serializer, ProductEndpoint.ProductDataFile);

		// repeating products so there is enough for pagination
		if (products is not null)
		{
			products =
				(
					from i in Enumerable.Range(0, 10)
					from x in products
					select x with
					{
						Name = i > 0 ? $"{x.Name} (+{i})" : x.Name,
						LongName = x.LongName != null && i > 1 ? $"{x.LongName} (+{i})" : x.LongName,
					}
				)
				.Select((x, i) => x with { ProductId = i })
				.ToArray();
		}

		var deals = products
			?.Where(p => !string.IsNullOrWhiteSpace(p.Discount))
			.Select(data => new Product(data, isFavorite: /* TODO */ false));

		return deals ?? Enumerable.Empty<Product>();
	}

	public async ValueTask<IImmutableList<Product>> GetAll(CancellationToken ct)
	{
		return (await GetDeals()).ToImmutableList();
	}

	public async ValueTask<IImmutableList<Product>> GetPaginated(CancellationToken ct, int from, int size)
	{
		var deals = await GetDeals();
		var paginated = deals
			.Skip(from)
			.Take(size)
			.ToImmutableList();

		return paginated;
	}
}
