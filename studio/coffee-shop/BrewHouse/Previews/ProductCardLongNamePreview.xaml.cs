using Microsoft.UI.Xaml.Controls;
using Uno.UI.HotDesign;

namespace BrewHouse.Previews;

// The product card with an overflowing name and description, to check the ellipsis / trimming. The
// ProductItem is built inline rather than taken from the catalogue.
[Preview("Product Card — Long Name", typeof(ContentControl), dataTemplateKey: "ProductCardTemplate")]
public sealed partial class ProductCardLongNamePreview : Preview
{
    public ProductCardLongNamePreview() => this.InitializeComponent();

    protected override object? LoadDataContext() => new ProductItem(
        "preview-long",
        "Extra-Hot Triple-Shot Oat-Milk Caramel Macchiato with Vanilla Cold Foam",
        "A deliberately long description to show how the card trims overflowing text to a single line with an ellipsis.",
        "Hot Drinks", "hot", "7.25", 7.25,
        CatalogData.AllProducts[0].ImageUrl,
        IsFeatured: false, IsSpecial: true);
}
