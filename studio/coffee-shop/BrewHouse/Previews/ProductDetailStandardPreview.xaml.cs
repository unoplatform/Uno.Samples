using Uno.UI.HotDesign;

namespace BrewHouse.Previews;

// The same ProductDetailPage as ProductDetailSpecialPreview, for a standard (non-special) product
// (ProductDetailPageMockData.Standard → the badge is hidden), supplied in XAML via {x:Bind}.
[Preview("Product — Standard", typeof(ProductDetailPage))]
public sealed partial class ProductDetailStandardPreview : Preview
{
    public ProductDetailStandardPreview() => this.InitializeComponent();
}
