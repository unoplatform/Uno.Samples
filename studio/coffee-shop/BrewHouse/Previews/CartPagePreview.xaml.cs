using Uno.UI.HotDesign;

namespace BrewHouse.Previews;

// CartPage with a small non-empty cart (CartPageMockData.Data) so the items list and order summary
// render.
[Preview("Cart", typeof(CartPage))]
public sealed partial class CartPagePreview : Preview
{
    public CartPagePreview() => this.InitializeComponent();

    protected override object? LoadDataContext() => CartPageMockData.Data;
}
