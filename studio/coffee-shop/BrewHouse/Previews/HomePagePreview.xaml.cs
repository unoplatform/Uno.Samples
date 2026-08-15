using Uno.UI.HotDesign;

namespace BrewHouse.Previews;

// HomePage with the carousel, specials, featured grid and cart summary populated from the sample's
// design-time mock (HomePageMockData.Data).
[Preview("Home", typeof(HomePage))]
public sealed partial class HomePagePreview : Preview
{
    public HomePagePreview() => this.InitializeComponent();

    protected override object? LoadDataContext() => HomePageMockData.Data;
}
