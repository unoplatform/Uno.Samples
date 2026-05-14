using System.Collections.Immutable;

namespace BrewHouse.Services;

public interface IProductService
{
	ValueTask<IImmutableList<HeroBanner>> GetBannersAsync(CancellationToken ct);
	ValueTask<IImmutableList<ProductCategory>> GetCategoriesAsync(CancellationToken ct);
	ValueTask<IImmutableList<Product>> GetAllProductsAsync(CancellationToken ct);
	ValueTask<IImmutableList<Product>> GetSpecialsAsync(CancellationToken ct);
	ValueTask<IImmutableList<Product>> GetFeaturedAsync(CancellationToken ct);
}

public class ProductService : IProductService
{
	private static readonly IImmutableList<HeroBanner> Banners = ImmutableArray.Create(
		new HeroBanner(
			"banner-1",
			"https://images.pexels.com/photos/1002740/pexels-photo-1002740.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
			"Start Your Morning Right",
			"Freshly brewed specialties crafted with love, every single day."),
		new HeroBanner(
			"banner-2",
			"https://images.pexels.com/photos/131053/pexels-photo-131053.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
			"Artisan Cappuccinos",
			"Perfectly balanced espresso with velvety micro-foam."),
		new HeroBanner(
			"banner-3",
			"https://images.pexels.com/photos/261434/pexels-photo-261434.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
			"Single Origin Beans",
			"Ethically sourced from the world's finest coffee farms."));

	private static readonly IImmutableList<ProductCategory> Categories = ImmutableArray.Create(
		new ProductCategory("all", "All", "☕"),
		new ProductCategory("hot", "Hot Drinks", "🔥"),
		new ProductCategory("cold", "Cold Drinks", "🧊"),
		new ProductCategory("pastries", "Pastries", "🥐"));

	private static readonly IImmutableList<Product> Products = ImmutableArray.Create(
		new Product("p-001", "Classic Latte",
			"Smooth espresso with velvety steamed milk and a delicate foam layer.",
			"Hot Drinks", "hot", 5.50m,
			"https://images.pexels.com/photos/3646111/pexels-photo-3646111.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
			IsFeatured: true, IsSpecial: true),
		new Product("p-002", "Cappuccino",
			"Equal parts espresso, steamed milk and thick microfoam — a true Italian classic.",
			"Hot Drinks", "hot", 4.75m,
			"https://images.pexels.com/photos/1694874/pexels-photo-1694874.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
			IsFeatured: true, IsSpecial: false),
		new Product("p-003", "Flat White",
			"Ristretto shots with a thin layer of velvety micro-foam — bold and smooth.",
			"Hot Drinks", "hot", 4.50m,
			"https://images.pexels.com/photos/131053/pexels-photo-131053.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
			IsFeatured: false, IsSpecial: false),
		new Product("p-004", "Caramel Macchiato",
			"Layers of vanilla syrup, steamed milk, espresso and drizzled caramel.",
			"Hot Drinks", "hot", 5.75m,
			"https://images.pexels.com/photos/3646111/pexels-photo-3646111.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
			IsFeatured: false, IsSpecial: true),
		new Product("p-005", "Iced Matcha",
			"Premium ceremonial matcha blended with oat milk over crushed ice.",
			"Cold Drinks", "cold", 6.00m,
			"https://images.pexels.com/photos/18553404/pexels-photo-18553404.png?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
			IsFeatured: true, IsSpecial: false),
		new Product("p-006", "Cold Brew",
			"Steeped for 18 hours in cold water for a naturally sweet, low-acid brew.",
			"Cold Drinks", "cold", 5.00m,
			"https://images.pexels.com/photos/261434/pexels-photo-261434.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
			IsFeatured: false, IsSpecial: false),
		new Product("p-007", "Butter Croissant",
			"Flaky, golden-baked croissant with premium French butter. Freshly baked each morning.",
			"Pastries", "pastries", 3.25m,
			"https://images.pexels.com/photos/20212456/pexels-photo-20212456.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
			IsFeatured: true, IsSpecial: true),
		new Product("p-008", "Almond Muffin",
			"Moist almond muffin with a crunchy streusel topping and hint of vanilla.",
			"Pastries", "pastries", 2.95m,
			"https://images.pexels.com/photos/20212456/pexels-photo-20212456.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
			IsFeatured: false, IsSpecial: false));

	public ValueTask<IImmutableList<HeroBanner>> GetBannersAsync(CancellationToken ct)
		=> new(Banners);

	public ValueTask<IImmutableList<ProductCategory>> GetCategoriesAsync(CancellationToken ct)
		=> new(Categories);

	public ValueTask<IImmutableList<Product>> GetAllProductsAsync(CancellationToken ct)
		=> new(Products);

	public ValueTask<IImmutableList<Product>> GetSpecialsAsync(CancellationToken ct)
		=> new(Products.Where(p => p.IsSpecial).ToImmutableList());

	public ValueTask<IImmutableList<Product>> GetFeaturedAsync(CancellationToken ct)
		=> new(Products.Where(p => p.IsFeatured).ToImmutableList());
}
