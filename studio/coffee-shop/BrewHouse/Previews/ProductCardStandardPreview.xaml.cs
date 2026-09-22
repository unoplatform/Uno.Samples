using Microsoft.UI.Xaml.Controls;
using Uno.UI.HotDesign;

namespace BrewHouse.Previews;

// The product card in its default data state (a standard, non-special product). The data is bound in
// XAML via {x:Bind}, so there is no LoadDataContext override. dataTemplateKey files it under Data
// Templates -> ProductCardTemplate.
[Preview("Product Card — Standard", typeof(ContentControl), dataTemplateKey: "ProductCardTemplate")]
public sealed partial class ProductCardStandardPreview : Preview
{
    public ProductCardStandardPreview() => this.InitializeComponent();
}
