namespace Commerce.Business;

public interface ICartService
{
	ValueTask Add(Product product, CancellationToken ct);

	IFeed<Cart> Cart { get; }

	ValueTask Update(Product product, uint quantity, CancellationToken ct);

	ValueTask Remove(Product product, CancellationToken ct);
}
