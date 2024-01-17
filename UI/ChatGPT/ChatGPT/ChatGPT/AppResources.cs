using Uno.Material;

namespace ChatGPT;

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
				.ColorOverrideDictionary()));

		// Load Uno.UI.Toolkit Resources
		this.Build(r => r.Merged(
			new ToolkitResources()));
	}
}
