using Uno.UI.HotDesign;

namespace BrewHouse.Previews;

// CartPage with a small non-empty cart (CartPageMockData.Data), supplied in XAML via {x:Bind}.
[Preview("Cart — Items", typeof(CartPage))]
public sealed partial class CartPagePreview : Preview
{
    public CartPagePreview() => this.InitializeComponent();
}
