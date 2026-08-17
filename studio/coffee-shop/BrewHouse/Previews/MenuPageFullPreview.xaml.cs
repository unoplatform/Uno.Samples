using Uno.UI.HotDesign;

namespace BrewHouse.Previews;

// MenuPage showing the full catalogue. The DataContext (MenuPageMockData.Data) is supplied in the
// XAML via {x:Bind}, so there is no LoadDataContext override here.
[Preview("Menu — Full Catalog", typeof(MenuPage))]
public sealed partial class MenuPageFullPreview : Preview
{
    public MenuPageFullPreview() => this.InitializeComponent();
}
