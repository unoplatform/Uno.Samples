namespace GrapeCityApp.Presentation;

public partial class MainViewModel : ObservableObject
{
	public MainViewModel(
		IStringLocalizer localizer,
		IOptions<AppConfig> appInfo)
	{
		Title = "Main";
		Title += $" - {localizer["ApplicationName"]}";
		Title += $" - {appInfo?.Value?.Environment}";
	}
	public string? Title { get; }

}
