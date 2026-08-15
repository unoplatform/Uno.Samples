using Uno.UI.HotDesign;

namespace BrewHouse.Previews;

// MenuPage showing the full catalogue. The page binds to a Model at runtime; here we feed it the
// same design-time mock DataContext the sample already ships (MenuPageMockData.Data), which mirrors
// the Model's binding surface with plain, materialized values so the grid and chip bar render.
[Preview("Menu — Full Catalog", typeof(MenuPage))]
public sealed partial class MenuPageFullPreview : Preview
{
    public MenuPageFullPreview() => this.InitializeComponent();

    protected override object? LoadDataContext() => MenuPageMockData.Data;
}
