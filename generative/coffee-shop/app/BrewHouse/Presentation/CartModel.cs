using Uno.Extensions.Reactive;

namespace BrewHouse.Presentation;

public partial record CartModel(ICartService CartService, IOrderService OrderService)
{
	public IListFeed<CartEntry> CartItems => ListFeed.AsyncEnumerable(CartService.ObserveItemsAsync);

	public IFeed<CartTotals> Totals => Feed.AsyncEnumerable(CartService.ObserveTotalsAsync);

	public IState<bool> IsOrderPlaced => State.Value(this, () => false);

	public async ValueTask IncrementItem(CartEntry item, CancellationToken ct)
	{
		await CartService.IncrementAsync(item.ProductId, ct);
		await IsOrderPlaced.Set(false, ct);
	}

	public async ValueTask DecrementItem(CartEntry item, CancellationToken ct)
	{
		await CartService.DecrementAsync(item.ProductId, ct);
		await IsOrderPlaced.Set(false, ct);
	}

	public async ValueTask PlaceOrder(CancellationToken ct)
	{
		var placedItems = await CartService.PlaceOrderAsync(ct);
		if (placedItems.Count > 0)
		{
			await OrderService.AddOrderAsync(placedItems, ct);
			await IsOrderPlaced.Set(true, ct);
		}
	}
}
