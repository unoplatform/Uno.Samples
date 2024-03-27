namespace Commerce.Business;

public class CartService : ICartService
{
	private IState<Cart> _cart => State<Cart>.Value(this, () => new Cart());

	public IFeed<Cart> Cart => _cart;

	public async ValueTask<Cart?> Get(CancellationToken ct)
		=> await _cart;

	public async ValueTask Add(Product product, CancellationToken ct)
		=> await _cart.Update(cart => (cart ?? new Cart()).Add(product), ct);

	public async ValueTask Update(Product product, uint quantity, CancellationToken ct)
		=> await _cart.Update(cart => (cart ?? new Cart()).Update(product, quantity), ct);

	public ValueTask Remove(Product product, CancellationToken ct)
		=> Update(product, 0, ct);
}
