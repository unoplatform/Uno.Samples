namespace Counter;

public sealed class AppResources : ResourceDictionary
{
    public AppResources()
    {
        // Load WinUI Resources
        this.Build(r => r.Merged(
            new XamlControlsResources()));
    }
}
