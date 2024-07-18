namespace Navigation.Presentation;

public partial class RequestValueMainViewModel : ObservableObject
{
	private INavigator _navigator;

	[ObservableProperty]
	private Entity entity;

	private bool _initialized = false;

	public RequestValueMainViewModel(INavigator navigator)
	{
		_navigator = navigator;
		Entity = new("[Not set]");
	}

	[RelayCommand]
	public async Task GoToSecondView()
	{
		Entity = await _navigator.GetDataAsync<Entity>(this);
	}
}
