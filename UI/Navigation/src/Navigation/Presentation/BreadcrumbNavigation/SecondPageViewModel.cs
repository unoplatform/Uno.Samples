namespace Navigation.Presentation;

public partial class SecondPageViewModel : BaseBreadcrumbViewModel
{
	private readonly INavigator _navigator;
	public SecondPageViewModel(IRouteNotifier notifier, INavigator navigator)
		: base(notifier, navigator)
	{
		_navigator = navigator;
	}
}
