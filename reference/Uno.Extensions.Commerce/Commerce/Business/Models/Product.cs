namespace Commerce.Business.Models;

public record Product()
{
	public Product(ProductData data, bool isFavorite)
		: this()
	{
		IsFavorite = isFavorite;
		ProductId = data.ProductId;
		Brand = data.Brand;
		Name = data.Name;
		LongName = data.LongName;
		Description = data.Description;
		Category = data.Category;
		FullPrice = data.FullPrice;
		Price = data.Price;
		Discount = data.Discount;
		Photo = data.Photo;
		Rating = data.Rating;
		//Reviews = (data.Reviews?.Select(data => new Review(data)).ToImmutableList()) ?? ImmutableList<Review>.Empty;
	}

	public int ProductId { get; init; }
	public string? Brand { get; init; }
	public string? Name { get; init; }
	public string? LongName { get; init; }
	public string? Description { get; init; }
	public string? Category { get; init; }
	public double? FullPrice { get; init; }
	public double? Price { get; init; }
	public double? Discount { get; init; }
	public string? Photo { get; init; }
	public double? Rating { get; init; }
	//public IImmutableList<Review>? Reviews { get; init; }

	public bool IsFavorite { get; init; }
	public double? DiscountedPrice => Price;
}
