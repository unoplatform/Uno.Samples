namespace ToDo;

// TODO: Extract these to uno extensions
// See https://github.com/unoplatform/uno.extensions/discussions/420
public interface IAppTheme
{
	bool IsDark { get; }

	Task SetThemeAsync(bool darkMode);
}
