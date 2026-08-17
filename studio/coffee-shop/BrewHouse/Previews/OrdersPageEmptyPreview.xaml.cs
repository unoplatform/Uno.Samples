using Uno.UI.HotDesign;

namespace BrewHouse.Previews;

// The same OrdersPage as OrdersPagePreview, with no orders (OrdersPageMockData.Empty), supplied in
// XAML via {x:Bind}.
[Preview("Orders — Empty", typeof(OrdersPage))]
public sealed partial class OrdersPageEmptyPreview : Preview
{
    public OrdersPageEmptyPreview() => this.InitializeComponent();
}
