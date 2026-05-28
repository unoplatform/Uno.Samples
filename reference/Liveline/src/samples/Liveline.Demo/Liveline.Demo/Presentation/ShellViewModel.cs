namespace Liveline.Demo.Presentation;

/// <summary>
/// Root navigation host VM. Navigates to the default Main route on construction.
/// </summary>
public class ShellViewModel
{
    public ShellViewModel(INavigator navigator)
    {
        _ = navigator.NavigateRouteAsync(this, "Main");
    }
}
