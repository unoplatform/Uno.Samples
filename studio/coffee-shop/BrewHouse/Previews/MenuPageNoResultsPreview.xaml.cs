using Uno.UI.HotDesign;

namespace BrewHouse.Previews;

// The same MenuPage as MenuPageFullPreview, in its empty-results state. The DataContext
// (MenuPageMockData.NoResults) is supplied in XAML via {x:Bind}, so there is no LoadDataContext.
[Preview("Menu — No Results", typeof(MenuPage))]
public sealed partial class MenuPageNoResultsPreview : Preview
{
    public MenuPageNoResultsPreview() => this.InitializeComponent();
}
