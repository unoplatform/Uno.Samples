using Microsoft.UI.Xaml.Controls;
using Uno.UI.HotDesign;

namespace BrewHouse.Previews;

// The product card in its default data state (a standard, non-special product). dataTemplateKey files
// this under Data Templates -> ProductCardTemplate; the same key is used in the XAML's ContentControl.
[Preview("Product Card — Standard", typeof(ContentControl), dataTemplateKey: "ProductCardTemplate")]
public sealed partial class ProductCardStandardPreview : Preview
{
    public ProductCardStandardPreview() => this.InitializeComponent();

    protected override object? LoadDataContext() => CatalogData.AllProducts[1]; // Cappuccino (not special)
}
