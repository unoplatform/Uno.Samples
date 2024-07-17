namespace Navigation.Presentation;

public partial class RequestValueMainViewModel : ObservableObject
{
	private INavigator _navigator;

	[ObservableProperty]
	private Entity? entity = new("[Not set]");

	public RequestValueMainViewModel(INavigator navigator)
	{
		_navigator = navigator;
	}

	public ICommand GoToSecondPage => new AsyncRelayCommand(GoToSecondView);

	public async Task GoToSecondView()
	{
		// FIXME: UI not being updated on Windows
		Entity = await _navigator.GetDataAsync<Entity>(this);
	}
}
