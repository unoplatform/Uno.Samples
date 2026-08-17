using Microsoft.UI.Xaml.Controls;
using Uno.UI.HotDesign;

namespace BrewHouse.Previews;

// The product card for a "Today's Special" product (badge visible). Data bound in XAML via {x:Bind}.
[Preview("Product Card — Today's Special", typeof(ContentControl), dataTemplateKey: "ProductCardTemplate")]
public sealed partial class ProductCardSpecialPreview : Preview
{
    public ProductCardSpecialPreview() => this.InitializeComponent();
}
