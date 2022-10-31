namespace Commerce.Data;

public class ProductEndpoint : IProductEndpoint
{
	public const string ProductDataFile = "products.json";
	private const string ReviewDataFile = "reviews.json";

	private readonly IStorage _dataService;
	private readonly ISerializer _serializer;

	public ProductEndpoint(IStorage dataService, ISerializer serializer)
	{
		_dataService = dataService;
		_serializer = serializer;
	}

	public async ValueTask<ProductData[]> GetAll(CancellationToken ct)
	{
		var products = await _dataService.ReadPackageFileAsync<ProductData[]>(_serializer, ProductDataFile);

		return products ?? Array.Empty<ProductData>();
	}

	public async ValueTask<ReviewData[]> GetReviews(int productId, CancellationToken ct)
	{
		var reviews = await _dataService.ReadPackageFileAsync<ReviewData[]>(_serializer, ReviewDataFile);

		return reviews ?? Array.Empty<ReviewData>();
	}
}
