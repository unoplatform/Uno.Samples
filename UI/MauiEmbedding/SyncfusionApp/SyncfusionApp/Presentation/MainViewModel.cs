namespace SyncfusionApp.Presentation;

public partial class MainViewModel : ObservableObject
{
	private INavigator _navigator;

	[ObservableProperty]
	private string? name;

	private int count = 0;

	[ObservableProperty]
	private string counterText = "Press Me";

	public MainViewModel(
		IStringLocalizer localizer,
		IOptions<AppConfig> appInfo,
		INavigator navigator)
	{
		_navigator = navigator;
		Title = "Main";
		Title += $" - {localizer["ApplicationName"]}";
		Title += $" - {appInfo?.Value?.Environment}";
		Counter = new RelayCommand(OnCount);
	}
	public string? Title { get; }

	public ICommand Counter { get; }

	private void OnCount()
	{
		CounterText = ++count switch
		{
			1 => "Pressed Once!",
			_ => $"Pressed {count} times!"
		};
	}
}
