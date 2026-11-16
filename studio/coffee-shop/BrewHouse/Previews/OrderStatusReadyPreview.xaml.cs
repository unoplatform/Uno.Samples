using Microsoft.UI.Xaml.Controls;
using Uno.UI.HotDesign;

namespace BrewHouse.Previews;

// The order status badge in its "Ready for Pickup" state. Data bound in XAML via {x:Bind}.
[Preview("Order Status — Ready", typeof(ContentControl), dataTemplateKey: "OrderStatusBadgeTemplate")]
public sealed partial class OrderStatusReadyPreview : Preview
{
    public OrderStatusReadyPreview() => this.InitializeComponent();
}
