namespace Navigation.Presentation;

public partial class FirstPageViewModel : BaseBreadcrumbViewModel
{
	private readonly INavigator _navigator;
	public FirstPageViewModel(IRouteNotifier notifier, INavigator navigator)
		: base(notifier, navigator)
	{
		_navigator = navigator;
	}
}
