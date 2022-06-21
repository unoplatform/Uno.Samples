namespace Commerce.Business.Models;

public record Cart(IImmutableList<CartItem> Items)
{
	public Cart() : this(ImmutableList<CartItem>.Empty)
	{
	}

	public string SubTotal => "$350,97";
	public string Tax1 => "$15,75";
	public string Tax2 => "$38.57";
	public string Total => "$405,29";

	public Cart Add(Product product)
	{
		var item = Items.FirstOrDefault(item => item.Product.ProductId == product.ProductId);
		var updatedItems = item is null
			? Items.Add(new CartItem(product, 1))
			: Items.Replace(item, item with { Quantity = item.Quantity + 1 });

		return this with { Items = updatedItems };
	}

	public Cart? Update(Product product, uint quantity)
	{
		var item = Items.FirstOrDefault(item => item.Product.ProductId == product.ProductId);
		if (item is null && quantity is 0)
		{
			return this;
		}

		var items = (item, quantity) switch
		{
			(null, 0) => Items,
			(not null, 0) => Items.Remove(item),
			(null, _) => Items.Add(new CartItem(product, quantity)),
			(not null, _) => Items.Replace(item, item with { Quantity = quantity })
		};

		return items.Count > 0 ? this with { Items = items } : default;
	}

	public Cart? Remove(Product product)
		=> Update(product, 0);
}
