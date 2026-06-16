using Uno.Extensions.Navigation;

namespace UnoCRM;

/// <summary>
/// View model backing <see cref="Shell"/> (the navigation root that hosts the extended
/// splash screen). On startup it navigates to the "Main" route, which the framework injects
/// into the splash screen's content area, revealing it once loading completes.
/// </summary>
public class ShellViewModel
{
    private readonly INavigator _navigator;

    public ShellViewModel(INavigator navigator)
    {
        _navigator = navigator;
        _ = Start();
    }

    private async Task Start() =>
        await _navigator.NavigateRouteAsync(this, route: "Main");
}
