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
	private async Task GoToSecondView()
	{
		var newEntity = await _navigator.GetDataAsync<Entity>(this);

		if (newEntity is { })
		{
			Entity = newEntity;
		}
	}
}
