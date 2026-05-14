using Uno.Extensions.Reactive;
using Uno.Extensions.Reactive.Bindings;

namespace BrewHouse.Presentation;

public partial record HomeModel(IProductService ProductService, ICartService CartService, INavigator Navigator, IThemeService ThemeService)
{
	public IState<bool> IsDark => State.Value(this, () => ThemeService.IsDark);

	public async ValueTask ToggleTheme(CancellationToken ct)
	{
		var isDark = await IsDark;
		await ThemeService.SetThemeAsync(isDark ? AppTheme.Light : AppTheme.Dark);
		await IsDark.UpdateAsync(_ => !isDark, ct);
	}

	public IListFeed<HeroBanner> HeroBanners => ListFeed.Async(ProductService.GetBannersAsync);

	public IListFeed<Product> Specials => ListFeed.Async(ProductService.GetSpecialsAsync);

	public IListFeed<Product> FeaturedProducts => ListFeed.Async(ProductService.GetFeaturedAsync);

	public IFeed<CartSummary> Cart => Feed.AsyncEnumerable(CartService.ObserveSummaryAsync);

	public async ValueTask AddToCart(Product product, CancellationToken ct)
	{
		await CartService.AddAsync(product, ct);
	}

	public async ValueTask GoToCart(CancellationToken ct)
	{
		await Navigator.NavigateRouteAsync(this, "Cart", cancellation: ct);
	}

	public async ValueTask GoToMenu(CancellationToken ct)
	{
		await Navigator.NavigateRouteAsync(this, "Menu", cancellation: ct);
	}
}
