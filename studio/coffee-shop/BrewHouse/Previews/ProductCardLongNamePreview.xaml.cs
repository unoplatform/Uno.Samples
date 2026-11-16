using Microsoft.UI.Xaml.Controls;
using Uno.UI.HotDesign;

namespace BrewHouse.Previews;

// The product card with an overflowing name and description, to check the ellipsis / trimming. Data
// bound in XAML via {x:Bind} (PreviewData.LongNameProduct).
[Preview("Product Card — Long Name", typeof(ContentControl), dataTemplateKey: "ProductCardTemplate")]
public sealed partial class ProductCardLongNamePreview : Preview
{
    public ProductCardLongNamePreview() => this.InitializeComponent();
}
