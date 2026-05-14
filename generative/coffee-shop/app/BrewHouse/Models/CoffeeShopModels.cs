using System.Collections.Immutable;

namespace BrewHouse.Models;

public partial record HeroBanner(string Id, string ImageUrl, string Title, string Subtitle);

public partial record Product(
	string Id,
	string Name,
	string Description,
	string Category,
	string CategoryId,
	decimal Price,
	string ImageUrl,
	bool IsFeatured,
	bool IsSpecial)
{
	public string PriceFormatted => Price.ToString("F2");
}

public partial record ProductCategory(string Id, string Name, string Icon)
{
	public string DisplayName => $"{Icon} {Name}";

	public override string ToString() => DisplayName;
}

public partial record CartEntry(
	string ProductId,
	string Name,
	decimal Price,
	string ImageUrl,
	int Quantity)
{
	public decimal LineTotal => Price * Quantity;
	public string PriceFormatted => Price.ToString("F2");
	public string LineTotalFormatted => LineTotal.ToString("F2");
}

public partial record CartTotals(decimal Subtotal, decimal Tax, decimal Total, int ItemCount)
{
	public string SubtotalFormatted => Subtotal.ToString("F2");
	public string TaxFormatted => Tax.ToString("F2");
	public string TotalFormatted => Total.ToString("F2");
	public string ItemCountText => ItemCount == 1 ? "1 item" : $"{ItemCount} items";
	public bool HasItems => ItemCount > 0;
	public bool IsEmpty => ItemCount == 0;
}

public partial record CartSummary(int ItemCount, decimal Total)
{
	public string TotalFormatted => Total.ToString("F2");
	public bool HasItems => ItemCount > 0;
	public bool IsEmpty => ItemCount == 0;
}

public partial record OrderItem(string Name, int Quantity, decimal Price);

public enum OrderStatus { Preparing, ReadyForPickup, Completed }

public partial record Order(
	string Id,
	string PlacedAt,
	OrderStatus Status,
	decimal Total,
	IImmutableList<OrderItem> Items)
{
	public string TotalFormatted => Total.ToString("F2");

	public string StatusDisplay => Status switch
	{
		OrderStatus.ReadyForPickup => "Ready for Pickup",
		OrderStatus.Preparing => "Preparing",
		OrderStatus.Completed => "Completed",
		_ => "Unknown"
	};

	public string ItemSummary =>
		Items.Count == 0
			? "No items"
			: string.Join(", ", Items.Select(i => $"{i.Name} x{i.Quantity}"));
}
