namespace Navigation.Presentation;

public partial class RequestValueSecondViewModel : ObservableObject
{
	[ObservableProperty]
	private Entity entity;

	private INavigator _navigator;

	public RequestValueSecondViewModel(INavigator navigator)
	{
		_navigator = navigator;

		Entity = new("");
	}

	public List<Entity> Entities { get; } =
		[
			new Entity("Entity 1"),
			new Entity("Entity 2"),
			new Entity("Entity 3"),
			new Entity("Entity 4")
		];

	[RelayCommand]
	private async Task GoBackToView()
		=> await _navigator.NavigateBackWithResultAsync(this, data: Entity);
}
