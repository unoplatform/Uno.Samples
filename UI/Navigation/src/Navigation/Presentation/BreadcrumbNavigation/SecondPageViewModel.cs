namespace Navigation.Presentation;

public partial class SecondPageViewModel : ObservableObject
{
	private readonly IRouteNotifier _notifier;
	private readonly INavigator _navigator;

	[ObservableProperty]
	private ICollection<string> breadcrumbs;

	public SecondPageViewModel(IRouteNotifier notifier, INavigator navigator)
	{
		_notifier = notifier;
		_notifier.RouteChanged += RouteChanged;
		_navigator = navigator;

		breadcrumbs = [];
	}

	private void RouteChanged(object? sender, RouteChangedEventArgs e)
	{
		if (e.Navigator is FrameNavigator navigator)
		{
			if (navigator.FullRoute is { } fullRoute)
			{
				var paths = fullRoute.Path?.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();

				if (fullRoute.Base is { Length: > 0 })
				{
					paths?.Insert(0, fullRoute.Base);
				}

				if (paths is { Count: > 0 })
				{
					Breadcrumbs = paths;
				}
			}
		}
	}

	[RelayCommand]
	public async Task NavigateBreadcrumb(string route)
	{
		await _navigator.NavigateRouteAsync(this, route, qualifier: Qualifiers.ClearBackStack);
	}

}
