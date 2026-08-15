using Microsoft.UI.Xaml.Controls;
using Uno.UI.HotDesign;

namespace BrewHouse.Previews;

// A control-level preview (not a whole page): the branded "Add to Cart" button on its own. Passing
// styleKey files it under Controls -> Button -> BrewButtonStyle in the Previews tree.
[Preview("Add to Cart", typeof(Button), styleKey: "BrewButtonStyle")]
public sealed partial class AddToCartButtonPreview : Preview
{
    public AddToCartButtonPreview() => this.InitializeComponent();
}
