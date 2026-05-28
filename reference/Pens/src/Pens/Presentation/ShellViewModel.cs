using Pens.Services;
using Uno.Extensions.Navigation;

namespace Pens.Presentation;

/// <summary>
/// Root navigation host VM. Decides the initial route: the tab shell when a
/// player is already selected, otherwise the player-picker login gate.
/// </summary>
public class ShellViewModel
{
    public ShellViewModel(INavigator navigator, IPlayerIdentityService identity)
    {
        _ = navigator.NavigateRouteAsync(this, identity.IsLoggedIn ? "Main" : "Login");
    }
}
