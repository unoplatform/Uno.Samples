namespace QuoteCraft.Presentation;

/// <summary>
/// Settings' Appearance row (part of <see cref="SettingsModel"/>): 0 System (the default), 1 Light, 2 Dark. The
/// window's theme service applies and saves the choice. The row follows a choice made in the rail's menu too
/// (<see cref="ThemeChoiceNotifier"/>); either side changes the theme only when the choice differs from the saved one,
/// so the two never call each other in a loop.
/// </summary>
public partial record ThemeSettingsModel
{
    private readonly IThemeService _theme;
    private readonly ThemeChoiceNotifier _themeChoice;

    public ThemeSettingsModel(IThemeService theme, ThemeChoiceNotifier themeChoice)
    {
        _theme = theme;
        _themeChoice = themeChoice;
    }

    public IState<int> ThemeIndex => State<int>.AsyncEnumerable(this, WatchThemeIndexAsync)
        .ForEach(async (index, ct) =>
        {
            var choice = ToTheme(index);
            if (choice != _theme.Theme)
            {
                await _theme.SetThemeAsync(choice);
                _themeChoice.Raise();
            }
        });

    private static AppTheme ToTheme(int index) => index switch { 1 => AppTheme.Light, 2 => AppTheme.Dark, _ => AppTheme.System };

    private static int ToIndex(AppTheme theme) => theme switch { AppTheme.Light => 1, AppTheme.Dark => 2, _ => 0 };

    private async IAsyncEnumerable<int> WatchThemeIndexAsync([System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct)
    {
        var changed = new ChangeSignal();
        Action handler = changed.Raise;
        _themeChoice.Changed += handler;
        try
        {
            while (true)
            {
                yield return ToIndex(_theme.Theme);
                await changed.WaitAsync(ct);
            }
        }
        finally
        {
            _themeChoice.Changed -= handler;
        }
    }
}
