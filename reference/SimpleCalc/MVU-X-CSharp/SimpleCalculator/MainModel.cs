namespace SimpleCalculator;

public partial record MainModel
{
    public IState<bool> IsDark { get; }

    public IState<Calculator> Calculator { get; }

    public async ValueTask InputCommand(string key, CancellationToken ct)
            => await Calculator.Update(c => c?.Input(key), ct);

    public MainModel(IThemeService themeService)
    {
        Calculator = State.Value(this, () => new Calculator());
        IsDark = State.Value(this, () => themeService.IsDark);

        themeService.ThemeChanged += async (_, _) =>
		{
			// Retrieve the IsDark property whilst still on the UI thread
			var isDark = themeService.IsDark;
			await IsDark.Update(_ => isDark, CancellationToken.None);
		};

		IsDark.ForEachAsync(async (dark, ct) 
            => await themeService.SetThemeAsync(dark ? AppTheme.Dark : AppTheme.Light));
    }
}

