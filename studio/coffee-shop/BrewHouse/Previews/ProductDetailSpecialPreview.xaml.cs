using Uno.UI.HotDesign;

namespace BrewHouse.Previews;

// ProductDetailPage for a "Today's Special" product (ProductDetailPageMockData.Data), so the special
// badge is visible.
[Preview("Product — Special", typeof(ProductDetailPage))]
public sealed partial class ProductDetailSpecialPreview : Preview
{
    public ProductDetailSpecialPreview() => this.InitializeComponent();

    protected override object? LoadDataContext() => ProductDetailPageMockData.Data;
}
