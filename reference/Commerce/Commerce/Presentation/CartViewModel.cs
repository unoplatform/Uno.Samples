namespace Commerce.Presentation;
using Commerce.Business;
using Commerce.Business.Models;

public partial record CartViewModel(ICartService CartService)
{
	public IFeed<Cart> Cart => CartService.Cart;

	public async ValueTask Remove(CartItem item, CancellationToken ct)
		=> await CartService.Remove(item.Product, ct);

	public async ValueTask More(CartItem item, CancellationToken ct)
		=> await CartService.Update(item.Product, item.Quantity + 1, ct);

	public async ValueTask Less(CartItem item, CancellationToken ct)
		=> await CartService.Update(item.Product, item.Quantity - 1, ct);
}
