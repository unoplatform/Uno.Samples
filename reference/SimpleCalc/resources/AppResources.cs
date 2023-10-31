using Uno.Toolkit.UI;
using Uno.Material;

namespace SimpleCalculator;

public sealed class AppResources : ResourceDictionary
{
	public AppResources()
	{
		// Load WinUI Resources
		this.Build(r => r.Merged(
			new XamlControlsResources()));

		// Load Material Resources
		this.Build(r => r.Merged(
			new MaterialTheme()
				.ColorOverrideDictionary(new Styles.ColorPaletteOverride())));

		// Load Uno.UI.Toolkit Resources
		this.Build(r => r.Merged(
			new ToolkitResources()));
    }
}