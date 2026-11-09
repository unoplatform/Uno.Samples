using Microsoft.UI.Xaml.Controls;
using Uno.UI.HotDesign;

namespace BrewHouse.Previews;

// The order status badge in its "Preparing" state. Data bound in XAML via {x:Bind}.
[Preview("Order Status — Preparing", typeof(ContentControl), dataTemplateKey: "OrderStatusBadgeTemplate")]
public sealed partial class OrderStatusPreparingPreview : Preview
{
    public OrderStatusPreparingPreview() => this.InitializeComponent();
}
