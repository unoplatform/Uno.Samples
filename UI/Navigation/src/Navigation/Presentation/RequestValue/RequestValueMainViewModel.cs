namespace Navigation.Presentation;

public partial class RequestValueMainViewModel : ObservableObject
{
	private INavigator _navigator;

	[ObservableProperty]
	private Entity? entity;

	public RequestValueMainViewModel(INavigator navigator)
	{
		_navigator = navigator;

		Entity = new("[Not set]");
	}

	public ICommand GoToSecondPage => new AsyncRelayCommand(GoToSecondView);

	public async Task GoToSecondView()
		=> Entity = await _navigator.GetDataAsync<Entity>(this);
}
