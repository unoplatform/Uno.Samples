namespace Navigation.Presentation;

public class TabBarWithDataViewModel
{
	private INavigator _navigator;

	public TabBarWithDataViewModel(INavigator navigator)
	{
		_navigator = navigator;

		Entity = new("TabBar Entity");

		GoToFirstTab = new AsyncRelayCommand(GoToFirstView);
		GoToSecondTab = new AsyncRelayCommand(GoToSecondView);

		Initialize();
	}

	public Entity Entity { get; set; }

	public ICommand GoToFirstTab { get; }

	public ICommand GoToSecondTab { get; }

	private async void Initialize()
	{
		await GoToFirstView();
	}

	private async Task GoToFirstView()
	{
		await _navigator.NavigateRouteAsync(this, route: "./TBDataOne", data: Entity);
	}

	private async Task GoToSecondView()
	{
		await _navigator.NavigateRouteAsync(this, route: "./TBDataTwo", data: Entity);
	}

}
