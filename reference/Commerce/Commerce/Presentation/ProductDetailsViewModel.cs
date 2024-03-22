namespace Commerce.Presentation;
using Commerce.Business;
using Commerce.Business.Models;

public partial class ProductDetailsViewModel
{
	private readonly IProductService _products;
	private readonly ICartService _cart;

	public ProductDetailsViewModel(
		IProductService products,
		ICartService cart,
		Product product)
	{
		_products = products;
		_cart = cart;

		Product = State.Value(this, () => product);
	}

	public virtual IState<Product> Product { get; }

	public virtual IListFeed<Review> Reviews => Product
		.SelectAsync(async (p, ct) => await _products.GetReviews(p.ProductId, ct))
		.AsListFeed();

	public async ValueTask ToggleIsFavorite(CancellationToken ct)
	{
		if (await Product is { } product)
		{
			await _products.Update(product with { IsFavorite = !product.IsFavorite }, ct);
		}
	}

	public async ValueTask AddToCart(CancellationToken ct)
	{
		if (await Product is { } product)
		{
			await _cart.Add(product, ct);
		}
	}
}
