namespace Navigation.Presentation;

public partial class ThirdPageViewModel : BaseBreadcrumbViewModel
{
	private readonly INavigator _navigator;
	public ThirdPageViewModel(IRouteNotifier notifier, INavigator navigator)
		: base(notifier, navigator)
	{
		_navigator = navigator;
	}
}
