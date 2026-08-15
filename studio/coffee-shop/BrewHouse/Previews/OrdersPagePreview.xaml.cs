using Uno.UI.HotDesign;

namespace BrewHouse.Previews;

// OrdersPage with a seed order history (OrdersPageMockData.Data).
[Preview("Orders — History", typeof(OrdersPage))]
public sealed partial class OrdersPagePreview : Preview
{
    public OrdersPagePreview() => this.InitializeComponent();

    protected override object? LoadDataContext() => OrdersPageMockData.Data;
}
