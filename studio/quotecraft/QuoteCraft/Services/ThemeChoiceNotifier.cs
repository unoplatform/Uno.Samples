namespace QuoteCraft.Services;

/// <summary>
/// Says that the theme choice changed (the rail's menu or Settings). Each page gets its own IThemeService (it is
/// scoped), and one instance does not hear another's changes, so the shell would keep naming the old choice.
/// </summary>
public sealed class ThemeChoiceNotifier
{
    public event Action? Changed;

    public void Raise() => Changed?.Invoke();
}
