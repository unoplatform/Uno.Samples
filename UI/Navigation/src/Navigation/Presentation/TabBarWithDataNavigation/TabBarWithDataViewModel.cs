namespace Navigation.Presentation;

public partial class TabBarWithDataViewModel
{
	private INavigator _navigator;

	public TabBarWithDataViewModel(INavigator navigator)
	{
		_navigator = navigator;

		Entity = new("TabBar Entity");

		Initialize();
	}

	public Entity Entity { get; set; }

	private async void Initialize()
	{
		await GoToFirstTab();
	}

	[RelayCommand]
	private async Task GoToFirstTab() 
		=> await _navigator.NavigateRouteAsync(this, route: "./TBDataOne", data: Entity);
	
	[RelayCommand]
	private async Task GoToSecondTab()
		=> await _navigator.NavigateRouteAsync(this, route: "./TBDataTwo", data: Entity);
}
