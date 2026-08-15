using Uno.UI.HotDesign;

namespace BrewHouse.Previews;

// The same ProductDetailPage as ProductDetailSpecialPreview, for a standard (non-special) product
// (ProductDetailPageMockData.Standard), so the special badge is hidden.
[Preview("Product — Standard", typeof(ProductDetailPage))]
public sealed partial class ProductDetailStandardPreview : Preview
{
    public ProductDetailStandardPreview() => this.InitializeComponent();

    protected override object? LoadDataContext() => ProductDetailPageMockData.Standard;
}
