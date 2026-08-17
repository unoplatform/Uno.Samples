using Microsoft.UI.Xaml.Controls;
using Uno.UI.HotDesign;

namespace BrewHouse.Previews;

// The order status badge in its "Preparing" state.
[Preview("Order Status — Preparing", typeof(ContentControl), dataTemplateKey: "OrderStatusBadgeTemplate")]
public sealed partial class OrderStatusPreparingPreview : Preview
{
    public OrderStatusPreparingPreview() => this.InitializeComponent();

    protected override object? LoadDataContext() => CatalogData.SeedOrders[1]; // "Preparing"
}
