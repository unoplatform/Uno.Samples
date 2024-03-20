namespace Commerce.Business;

public interface IDealService
{
    ValueTask<IImmutableList<Product>> GetAll(CancellationToken ct);

    ValueTask<IImmutableList<Product>> GetPaginated(CancellationToken ct, int from, int size);
}
