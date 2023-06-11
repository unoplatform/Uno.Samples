namespace Commerce.ViewModels;

public partial class ProductsViewModel
{
	private readonly IProductService _products;

	public ProductsViewModel(
		IProductService products,
		Filters? filter)
	{
		_products = products;

		Filter = State.Value(this, () => filter);
	}

	public IState<string> Term => State<string>.Value(this, () => "");

	public IState<Filters?> Filter { get; }

	public IListFeed<Product> Items => Feed
		.Combine(Results, Filter)
		.Select(FilterProducts)
		.AsListFeed();

	private IFeed<IImmutableList<Product>> Results => Term
		.SelectAsync(_products.Search);

	private IImmutableList<Product> FilterProducts((IImmutableList<Product> products, Filters? filter) inputs)
		=> inputs.products.Where(p => inputs.filter?.Match(p) ?? true).ToImmutableList();
}
