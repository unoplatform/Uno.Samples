using Uno.Extensions.Toolkit;
using Uno.Toolkit.UI;

namespace SimpleCalculator.ThemeService;
public class AppThemeService : IThemeService
{   
    private readonly UIElement _element;

    public AppThemeService(UIElement element)
    {
        _element = element;
    }

    public bool IsDark => SystemThemeHelper.IsRootInDarkMode(_element.XamlRoot!);

    public async Task SetThemeAsync(AppTheme theme)
    {
        var tcs = new TaskCompletionSource<object?>();
        _element.DispatcherQueue.TryEnqueue(() =>
        {
            SystemThemeHelper.SetRootTheme(_element.XamlRoot, theme == AppTheme.Dark);
            DesiredThemeChanged?.Invoke(this, theme);
#nullable disable
            tcs.TrySetResult(default);
#nullable restore
        });
            await tcs.Task;
    }
    public AppTheme Theme => IsDark ? AppTheme.Dark : AppTheme.Light;

    public event EventHandler<AppTheme>? DesiredThemeChanged;
}
