using Microsoft.UI.Xaml.Controls;
using Uno.UI.HotDesign;

namespace BrewHouse.Previews;

// The order status badge in its "Completed" state.
[Preview("Order Status — Completed", typeof(ContentControl), dataTemplateKey: "OrderStatusBadgeTemplate")]
public sealed partial class OrderStatusCompletedPreview : Preview
{
    public OrderStatusCompletedPreview() => this.InitializeComponent();

    protected override object? LoadDataContext() => CatalogData.SeedOrders[2]; // "Completed"
}
