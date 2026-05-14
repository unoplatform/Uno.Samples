using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace BrewHouse.Services;

public interface IOrderService
{
	IAsyncEnumerable<IImmutableList<Order>> ObserveOrdersAsync(CancellationToken ct);
	ValueTask AddOrderAsync(IImmutableList<CartEntry> cartItems, CancellationToken ct);
}

public class OrderService : IOrderService
{
	private const string OrderIdPrefix = "ORD-";

	private readonly object _lock = new();
	private int _orderCounter;
	private ImmutableList<Order> _orders;
	private TaskCompletionSource _changed = new(TaskCreationOptions.RunContinuationsAsynchronously);

	public OrderService()
	{
		_orders = ImmutableList.Create(
			new Order("ORD-1042", "Today, 9:14 AM", OrderStatus.ReadyForPickup, 12.25m,
				ImmutableArray.Create(
					new OrderItem("Latte", 1, 5.50m),
					new OrderItem("Croissant", 2, 3.25m))),
			new Order("ORD-1041", "Today, 8:02 AM", OrderStatus.Preparing, 10.75m,
				ImmutableArray.Create(
					new OrderItem("Cappuccino", 1, 4.75m),
					new OrderItem("Iced Matcha", 1, 6.00m))),
			new Order("ORD-1039", "Yesterday, 3:45 PM", OrderStatus.Completed, 5.50m,
				ImmutableArray.Create(
					new OrderItem("Latte", 1, 5.50m))));

		_orderCounter = 1042;
	}

	private void NotifyChanged()
	{
		var old = Interlocked.Exchange(ref _changed, new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously));
		old.TrySetResult();
	}

	public async IAsyncEnumerable<IImmutableList<Order>> ObserveOrdersAsync(
		[EnumeratorCancellation] CancellationToken ct)
	{
		while (!ct.IsCancellationRequested)
		{
			ImmutableList<Order> snapshot;
			Task waitTask;
			lock (_lock)
			{
				snapshot = _orders;
				waitTask = _changed.Task;
			}

			yield return snapshot;
			await waitTask.WaitAsync(ct);
		}
	}

	public ValueTask AddOrderAsync(IImmutableList<CartEntry> cartItems, CancellationToken ct)
	{
		if (cartItems.Count == 0)
		{
			return ValueTask.CompletedTask;
		}

		var orderItems = cartItems.Select(ci => new OrderItem(ci.Name, ci.Quantity, ci.Price)).ToImmutableArray();
		var total = cartItems.Sum(ci => ci.LineTotal);

		lock (_lock)
		{
			_orderCounter++;
			var order = new Order(
				$"{OrderIdPrefix}{_orderCounter}",
				"Just now",
				OrderStatus.Preparing,
				total,
				orderItems);

			_orders = _orders.Insert(0, order);
		}

		NotifyChanged();
		return ValueTask.CompletedTask;
	}
}
