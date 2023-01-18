using Uno.Toolkit.UI;

namespace SimpleCalculator.ThemeService;
public class AppThemeService : IAppThemeService
{
    private static IAppThemeService? _instance;
    
    public static IAppThemeService Instance => _instance ?? throw new Exception("You must call 'AppThemeService.Init(window)' prior to getting the current instance.");
    
    public static IAppThemeService Init(Window window) =>
        new AppThemeService(window);
    
    private readonly Window _window;

    private AppThemeService(Window window)
    {
        _instance = this;
        _window = window;
    }

    public bool IsDark => SystemThemeHelper.IsRootInDarkMode(_window.Content.XamlRoot!);

    public async ValueTask SetThemeAsync(bool darkMode, CancellationToken ct)
    {
        var tcs = new TaskCompletionSource<object?>();
        await using var _ = ct.Register(() => tcs.TrySetCanceled());
        _window.DispatcherQueue.TryEnqueue(() =>
        {
            if (!ct.IsCancellationRequested)
            {
                SystemThemeHelper.SetRootTheme(_window.Content.XamlRoot, darkMode);
            }
            tcs.TrySetResult(default);
        });
        await tcs.Task;
    }
}
