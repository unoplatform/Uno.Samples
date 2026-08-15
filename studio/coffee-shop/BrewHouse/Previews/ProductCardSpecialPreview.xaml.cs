using Microsoft.UI.Xaml.Controls;
using Uno.UI.HotDesign;

namespace BrewHouse.Previews;

// The product card for a "Today's Special" product, so the badge is visible. Same DataTemplate as
// the other card previews, differing only in the ProductItem returned below.
[Preview("Product Card — Today's Special", typeof(ContentControl), dataTemplateKey: "ProductCardTemplate")]
public sealed partial class ProductCardSpecialPreview : Preview
{
    public ProductCardSpecialPreview() => this.InitializeComponent();

    protected override object? LoadDataContext() => CatalogData.AllProducts[0]; // Classic Latte (special)
}
