namespace ChatGPT;

public sealed class AppResources : ResourceDictionary
{
    public AppResources()
    {
        // Load WinUI Resources
        this.Build(r => r.Merged(
            new XamlControlsResources()));

        // Load Uno.UI.Toolkit Resources
        this.Build(r => r.Merged(
            new ToolkitResources()));
    }
}
