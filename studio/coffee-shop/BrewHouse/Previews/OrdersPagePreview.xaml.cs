using Uno.UI.HotDesign;

namespace BrewHouse.Previews;

// OrdersPage with a seed order history (OrdersPageMockData.Data), supplied in XAML via {x:Bind}.
[Preview("Orders — History", typeof(OrdersPage))]
public sealed partial class OrdersPagePreview : Preview
{
    public OrdersPagePreview() => this.InitializeComponent();
}
