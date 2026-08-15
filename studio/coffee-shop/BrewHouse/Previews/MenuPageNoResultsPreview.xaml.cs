using Uno.UI.HotDesign;

namespace BrewHouse.Previews;

// The same MenuPage as MenuPageFullPreview, previewed in its empty-results state. Two previews over
// the same typeof(MenuPage) is how you get several previews of one page, each in a distinct data
// state — here the difference is entirely in the DataContext returned below.
[Preview("Menu — No Results", typeof(MenuPage))]
public sealed partial class MenuPageNoResultsPreview : Preview
{
    public MenuPageNoResultsPreview() => this.InitializeComponent();

    protected override object? LoadDataContext() => MenuPageMockData.NoResults;
}
