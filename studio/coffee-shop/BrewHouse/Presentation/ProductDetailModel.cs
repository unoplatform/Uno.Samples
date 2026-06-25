using BrewHouse.Presentation.MockData;
using BrewHouse.Presentation.Services;

namespace BrewHouse.Presentation;

// Bound to a single product via DataViewMap<ProductDetailPage, ProductDetailModel, ProductItem>:
// Navigation passes the tapped ProductItem as the record's first parameter and resolves the shared
// CartService + INavigator from DI.
public partial record ProductDetailModel(ProductItem Product, ICartService Cart, INavigator Navigator)
{
    public async ValueTask AddToCart(CancellationToken ct)
        => await Cart.AddToCartAsync(Product, ct);

    public async ValueTask GoBack(CancellationToken ct)
        => await Navigator.NavigateBackAsync(this, cancellation: ct);
}
