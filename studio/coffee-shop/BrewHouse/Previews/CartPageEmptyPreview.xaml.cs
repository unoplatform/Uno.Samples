using Uno.UI.HotDesign;

namespace BrewHouse.Previews;

// The same CartPage as CartPagePreview, with an empty cart (CartPageMockData.Empty), supplied in
// XAML via {x:Bind}.
[Preview("Cart — Empty", typeof(CartPage))]
public sealed partial class CartPageEmptyPreview : Preview
{
    public CartPageEmptyPreview() => this.InitializeComponent();
}
