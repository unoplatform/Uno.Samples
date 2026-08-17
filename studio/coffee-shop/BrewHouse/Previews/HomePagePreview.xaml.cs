using Uno.UI.HotDesign;

namespace BrewHouse.Previews;

// HomePage populated from HomePageMockData.Data, supplied in XAML via {x:Bind}.
[Preview("Home", typeof(HomePage))]
public sealed partial class HomePagePreview : Preview
{
    public HomePagePreview() => this.InitializeComponent();
}
