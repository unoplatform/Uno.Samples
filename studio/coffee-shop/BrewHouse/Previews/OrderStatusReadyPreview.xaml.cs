using Microsoft.UI.Xaml.Controls;
using Uno.UI.HotDesign;

namespace BrewHouse.Previews;

// The order status badge in its "Ready for Pickup" state. Same shared OrderStatusBadgeTemplate as the
// other status previews, differing only in the OrderRecord returned below.
[Preview("Order Status — Ready", typeof(ContentControl), dataTemplateKey: "OrderStatusBadgeTemplate")]
public sealed partial class OrderStatusReadyPreview : Preview
{
    public OrderStatusReadyPreview() => this.InitializeComponent();

    protected override object? LoadDataContext() => CatalogData.SeedOrders[0]; // "Ready for Pickup"
}
