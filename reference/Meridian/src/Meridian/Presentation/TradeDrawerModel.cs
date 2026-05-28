namespace Meridian.Presentation;

public partial record TradeDrawerModel
{
	public IState<string> Side => State.Value(this, () => "buy");
	public IState<int> Quantity => State.Value(this, () => 0);
	public IState<string> OrderType => State.Value(this, () => "market");
	public IState<decimal> LimitPrice => State.Value(this, () => 0m);
	public IState<bool> IsConfirmed => State.Value(this, () => false);

	public async ValueTask SetQuantity(int qty) =>
		await Quantity.Set(qty, CancellationToken.None);

	public async ValueTask SelectOrderType(string type)
	{
		await OrderType.Set(type, CancellationToken.None);
		if (type == "market")
			await LimitPrice.Set(0m, CancellationToken.None);
	}

	public async ValueTask SubmitOrder() =>
		await IsConfirmed.Set(true, CancellationToken.None);

	public async ValueTask Reset()
	{
		await Side.Set("buy", CancellationToken.None);
		await Quantity.Set(0, CancellationToken.None);
		await OrderType.Set("market", CancellationToken.None);
		await LimitPrice.Set(0m, CancellationToken.None);
		await IsConfirmed.Set(false, CancellationToken.None);
	}
}
