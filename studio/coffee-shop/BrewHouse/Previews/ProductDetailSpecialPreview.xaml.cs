using Uno.UI.HotDesign;

namespace BrewHouse.Previews;

// ProductDetailPage for a "Today's Special" product (ProductDetailPageMockData.Data → the badge is
// visible), supplied in XAML via {x:Bind}.
[Preview("Product — Special", typeof(ProductDetailPage))]
public sealed partial class ProductDetailSpecialPreview : Preview
{
    public ProductDetailSpecialPreview() => this.InitializeComponent();
}
