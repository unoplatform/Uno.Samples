using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace BrewHouse.Services;

public interface ICartService
{
	ValueTask<IImmutableList<CartEntry>> GetItemsAsync(CancellationToken ct);
	IAsyncEnumerable<IImmutableList<CartEntry>> ObserveItemsAsync(CancellationToken ct);
	IAsyncEnumerable<CartSummary> ObserveSummaryAsync(CancellationToken ct);
	IAsyncEnumerable<CartTotals> ObserveTotalsAsync(CancellationToken ct);
	ValueTask AddAsync(Product product, CancellationToken ct);
	ValueTask IncrementAsync(string productId, CancellationToken ct);
	ValueTask DecrementAsync(string productId, CancellationToken ct);
	ValueTask<IImmutableList<CartEntry>> PlaceOrderAsync(CancellationToken ct);
}

public class CartService : ICartService
{
	private const decimal TaxRate = 0.08m;

	private readonly object _lock = new();
	private ImmutableList<CartEntry> _items = ImmutableList<CartEntry>.Empty;
	private TaskCompletionSource _changed = new(TaskCreationOptions.RunContinuationsAsynchronously);

	private void NotifyChanged()
	{
		var old = Interlocked.Exchange(ref _changed, new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously));
		old.TrySetResult();
	}

	public ValueTask<IImmutableList<CartEntry>> GetItemsAsync(CancellationToken ct)
	{
		lock (_lock)
		{
			return new(_items);
		}
	}

	public async IAsyncEnumerable<IImmutableList<CartEntry>> ObserveItemsAsync(
		[EnumeratorCancellation] CancellationToken ct)
	{
		while (!ct.IsCancellationRequested)
		{
			ImmutableList<CartEntry> snapshot;
			Task waitTask;
			lock (_lock)
			{
				snapshot = _items;
				waitTask = _changed.Task;
			}

			yield return snapshot;
			await waitTask.WaitAsync(ct);
		}
	}

	public async IAsyncEnumerable<CartSummary> ObserveSummaryAsync(
		[EnumeratorCancellation] CancellationToken ct)
	{
		await foreach (var items in ObserveItemsAsync(ct))
		{
			yield return new CartSummary(
				items.Sum(i => i.Quantity),
				items.Sum(i => i.LineTotal));
		}
	}

	public async IAsyncEnumerable<CartTotals> ObserveTotalsAsync(
		[EnumeratorCancellation] CancellationToken ct)
	{
		await foreach (var items in ObserveItemsAsync(ct))
		{
			var subtotal = items.Sum(i => i.LineTotal);
			var tax = Math.Round(subtotal * TaxRate, 2);
			yield return new CartTotals(subtotal, tax, subtotal + tax, items.Sum(i => i.Quantity));
		}
	}

	public ValueTask AddAsync(Product product, CancellationToken ct)
	{
		lock (_lock)
		{
			var existing = _items.FirstOrDefault(i => i.ProductId == product.Id);
			if (existing is not null)
			{
				_items = _items.Replace(existing, existing with { Quantity = existing.Quantity + 1 });
			}
			else
			{
				_items = _items.Add(new CartEntry(product.Id, product.Name, product.Price, product.ImageUrl, 1));
			}
		}

		NotifyChanged();
		return ValueTask.CompletedTask;
	}

	public ValueTask IncrementAsync(string productId, CancellationToken ct)
	{
		lock (_lock)
		{
			var existing = _items.FirstOrDefault(i => i.ProductId == productId);
			if (existing is not null)
			{
				_items = _items.Replace(existing, existing with { Quantity = existing.Quantity + 1 });
			}
		}

		NotifyChanged();
		return ValueTask.CompletedTask;
	}

	public ValueTask DecrementAsync(string productId, CancellationToken ct)
	{
		lock (_lock)
		{
			var existing = _items.FirstOrDefault(i => i.ProductId == productId);
			if (existing is null)
			{
				return ValueTask.CompletedTask;
			}

			if (existing.Quantity > 1)
			{
				_items = _items.Replace(existing, existing with { Quantity = existing.Quantity - 1 });
			}
			else
			{
				_items = _items.Remove(existing);
			}
		}

		NotifyChanged();
		return ValueTask.CompletedTask;
	}

	public ValueTask<IImmutableList<CartEntry>> PlaceOrderAsync(CancellationToken ct)
	{
		ImmutableList<CartEntry> placedItems;
		lock (_lock)
		{
			placedItems = _items;
			_items = ImmutableList<CartEntry>.Empty;
		}

		NotifyChanged();
		return new(placedItems);
	}
}
