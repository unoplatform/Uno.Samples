using Uno.Extensions.Navigation;

namespace EnterpriseDashboard.Views;

/// <summary>
/// Root navigation host VM. Pushes the chrome shell (ShellPage) as the initial route.
/// </summary>
public class ShellViewModel
{
    public ShellViewModel(INavigator navigator)
    {
        _ = navigator.NavigateRouteAsync(this, "Main");
    }
}
