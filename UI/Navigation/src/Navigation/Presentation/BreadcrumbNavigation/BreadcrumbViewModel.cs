using System.Collections.ObjectModel;

namespace Navigation.Presentation;

public partial class BreadcrumbViewModel : ObservableObject
{
	private readonly IRouteNotifier _notifier;
	private readonly INavigator _navigator;

	[ObservableProperty]
	private ICollection<string> _breadcrumbs = new List<string>();

	public BreadcrumbViewModel(IRouteNotifier notifier, INavigator navigator)
	{
		_notifier = notifier;
		_notifier.RouteChanged += RouteChanged;
		_navigator = navigator;
	}

	private void RouteChanged(object? sender, RouteChangedEventArgs e)
	{
		if(e.Navigator is FrameNavigator navigator)
		{
			if (navigator.FullRoute is { } fullRoute)
			{
				var paths = fullRoute.Path?.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();

				if(fullRoute.Base is { Length: > 0 })
				{
					paths?.Insert(0, fullRoute.Base);
				}

				Breadcrumbs = new ObservableCollection<string>(paths);
			}
		}
	}

	[RelayCommand]
	public async Task NavigateToFirst()
	{
		await _navigator.NavigateViewAsync<FirstBreadcrumbPage>(this);
	}

	[RelayCommand]
	public async Task NavigateBreadcrumb()
	{

	}
}
