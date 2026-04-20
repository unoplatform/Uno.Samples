namespace GridWatch.Presentation;

public class ShellModel
{
    private readonly INavigator _navigator;

    public ShellModel(
        INavigator navigator)
    {
        _navigator = navigator;
        _ = navigator.NavigateRouteAsync(this, "Login");
    }
}
