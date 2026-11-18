namespace Navigation.Presentation;

public partial class TabBarWithDataViewModel
{
	private INavigator _navigator;

	public TabBarWithDataViewModel(INavigator navigator)
	{
		_navigator = navigator;

		Entity = new("TabBar Entity");
	}

	public Entity Entity { get; set; }
}
