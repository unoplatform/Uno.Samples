using Uno.UI.HotDesign;

namespace BrewHouse.Previews;

// The same CartPage as CartPagePreview, previewed with an empty cart so the "empty cart" hero shows.
// Two previews over the same typeof(CartPage), differing only in the DataContext returned below.
[Preview("Cart — Empty", typeof(CartPage))]
public sealed partial class CartPageEmptyPreview : Preview
{
    public CartPageEmptyPreview() => this.InitializeComponent();

    protected override object? LoadDataContext() => CartPageMockData.Empty;
}
