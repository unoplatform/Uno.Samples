namespace Voyago.Presentation;

// Backs the root Shell (the ExtendedSplashScreen host). The default nested route (Main) is
// navigated automatically; this model just holds the navigator for any future shell-level nav.
public class ShellModel
{
    private readonly INavigator _navigator;

    public ShellModel(INavigator navigator)
    {
        _navigator = navigator;
    }
}
