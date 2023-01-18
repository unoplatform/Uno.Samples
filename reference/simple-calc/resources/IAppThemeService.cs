namespace SimpleCalculator.ThemeService;

public interface IAppThemeService
{
    bool IsDark { get; }
    ValueTask SetThemeAsync(bool darkMode, CancellationToken ct);
}
