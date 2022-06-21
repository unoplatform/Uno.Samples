namespace Commerce.Business;

public interface IDealService
{
	ValueTask<IImmutableList<Product>> GetAll(CancellationToken ct);
}
