using System.Windows.Input;

namespace BrewHouse.Presentation.MockData;

// Bound to a single product via DataViewMap<ProductDetailPage, ProductDetailModel, ProductItem>:
// Navigation passes the tapped ProductItem as the ctor's data argument and resolves AppState +
// INavigator from DI.
public class ProductDetailModel
{
    private readonly AppState _state;
    private readonly INavigator? _navigator;

    public ProductDetailModel(ProductItem product, AppState state, INavigator? navigator = null)
    {
        Product = product;
        _state = state;
        _navigator = navigator;

        AddToCartCommand = new RelayCommand(() => _state.AddToCart(Product));
        GoBackCommand = new RelayCommand(() => _ = _navigator?.NavigateBackAsync(this));
    }

    public ProductItem Product { get; }

    public ICommand AddToCartCommand { get; }
    public ICommand GoBackCommand { get; }
}
