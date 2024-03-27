namespace Commerce.ViewModels;

public record Filters(bool Shoes, bool Accessories, bool Headwear, bool InStockOnly)
{
	public bool Match(Product product)
	{
		if (!Shoes && !Accessories && !Headwear)
		{
			return true;
		}

		if (Shoes && product.Category?.IndexOf("Shoes", StringComparison.OrdinalIgnoreCase) >= 0)
		{
			return true;
		}

		if (Accessories && product.Category?.IndexOf("Accessories", StringComparison.OrdinalIgnoreCase) >= 0)
		{
			return true;
		}

		if (Headwear && product.Category?.IndexOf("Headwear", StringComparison.OrdinalIgnoreCase) >= 0)
		{
			return true;
		}

		return false;
	}
}
