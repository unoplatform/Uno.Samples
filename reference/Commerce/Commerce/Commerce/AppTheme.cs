namespace Commerce;

// TODO: Extract these to uno extensions
// See https://github.com/unoplatform/uno.extensions/discussions/420
public class AppTheme : IAppTheme
{
    private readonly Window _window;
    private readonly IDispatcher _dispatcher;
    public AppTheme(Window window, IDispatcher dispatcher)
    {
        _window = window;
        _dispatcher = dispatcher;
    }
    public bool IsDark => _window.Content?.XamlRoot is { } root ? SystemThemeHelper.IsRootInDarkMode(root) : false;

    public async Task SetThemeAsync(bool darkMode)
    {
        await _dispatcher.ExecuteAsync(() =>
        {
            SystemThemeHelper.SetRootTheme(_window.Content?.XamlRoot, darkMode);
        });
    }
}
