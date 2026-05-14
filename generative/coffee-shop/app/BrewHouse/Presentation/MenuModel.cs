using Uno.Extensions.Reactive;

namespace BrewHouse.Presentation;

public partial record MenuModel(IProductService ProductService, ICartService CartService)
{
	public IState<string> SelectedCategory => State.Value(this, () => "all");

	public IListFeed<Product> FilteredProducts =>
		SelectedCategory
			.SelectAsync(async (category, ct) =>
			{
				var all = await ProductService.GetAllProductsAsync(ct);
				var id = category ?? "all";
				return id == "all"
					? all
					: (IImmutableList<Product>)all.Where(p => p.CategoryId == id).ToImmutableList();
			})
			.AsListFeed();

	public async ValueTask FilterAll(CancellationToken ct)
		=> await SelectedCategory.Set("all", ct);

	public async ValueTask FilterHot(CancellationToken ct)
		=> await SelectedCategory.Set("hot", ct);

	public async ValueTask FilterCold(CancellationToken ct)
		=> await SelectedCategory.Set("cold", ct);

	public async ValueTask FilterPastries(CancellationToken ct)
		=> await SelectedCategory.Set("pastries", ct);

	public async ValueTask AddToCart(Product product, CancellationToken ct)
	{
		await CartService.AddAsync(product, ct);
	}
}
