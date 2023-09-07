namespace TelerikApp.Presentation;

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
		GoToSecond = new AsyncRelayCommand(GoToSecondView);
		Counter = new RelayCommand(OnCount);
	}
	public string? Title { get; }

	public ICommand GoToSecond { get; }

	public ICommand Counter { get; }

	private async Task GoToSecondView()
	{
		await _navigator.NavigateViewModelAsync<SecondViewModel>(this, data: new Entity(Name!));
	}


	private void OnCount()
	{
		CounterText = ++count switch
		{
			1 => "Pressed Once!",
			_ => $"Pressed {count} times!"
		};
	}
}
