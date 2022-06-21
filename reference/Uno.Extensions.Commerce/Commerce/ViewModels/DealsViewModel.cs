using CommunityToolkit.Mvvm.Messaging;
using Uno.Extensions.Reactive.Messaging;

namespace Commerce.ViewModels;

public class DealsViewModel
{
	private readonly IDealService _dealService;
	private readonly IProductService _productService;

	private readonly Signal _refreshFavorites = new();

	public DealsViewModel(IDealService dealService, IProductService productService, IMessenger messenger)
	{
		_dealService = dealService;
		_productService = productService;

		messenger.Observe(Favorites, p => p.ProductId);
	}

	public IListFeed<Product> Items => ListFeed.Async(_dealService.GetAll);

	public IListState<Product> Favorites => ListState.Async(this, _productService.GetFavorites);

	public async ValueTask RemoveFromFavorite(Product product, CancellationToken ct)
		=> await _productService.Update(product with { IsFavorite = false }, ct);
}
