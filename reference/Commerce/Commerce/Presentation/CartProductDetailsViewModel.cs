namespace Commerce.ViewModels;


public partial class CartProductDetailsViewModel : ProductDetailsViewModel
{
	private readonly CartItem _cartItem;

	public CartProductDetailsViewModel(
		IProductService products,
		ICartService cart,
		CartItem cartItem)
		: base(products, cart, cartItem.Product)
	{
		_cartItem = cartItem;
	}

	public override IState<Product> Product => base.Product;

	public override IListFeed<Review> Reviews => base.Reviews;
}
