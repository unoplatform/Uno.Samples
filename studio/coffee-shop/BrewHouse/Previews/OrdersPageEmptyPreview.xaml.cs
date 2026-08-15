using Uno.UI.HotDesign;

namespace BrewHouse.Previews;

// The same OrdersPage as OrdersPagePreview, previewed with no orders so the "No orders yet" empty
// state shows. Two previews over the same typeof(OrdersPage), differing only in the DataContext
// returned below.
[Preview("Orders — Empty", typeof(OrdersPage))]
public sealed partial class OrdersPageEmptyPreview : Preview
{
    public OrdersPageEmptyPreview() => this.InitializeComponent();

    protected override object? LoadDataContext() => OrdersPageMockData.Empty;
}
