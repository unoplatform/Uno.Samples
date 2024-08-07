using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System;
using Uno.Extensions.Navigation;

namespace Navigation.Presentation;

public partial class BreadcrumbViewModel : INotifyPropertyChanged
{
	private readonly IRouteNotifier _notifier;
	private ObservableCollection<string> _breadcrumbs = new ObservableCollection<string>();

	public ObservableCollection<string> Breadcrumbs
	{
		get => _breadcrumbs;
		set
		{
			_breadcrumbs = value;
			OnPropertyChanged();
		}
	}

	public BreadcrumbViewModel(IRouteNotifier notifier, INavigator navigator)
	{
		_notifier = notifier;
		_notifier.RouteChanged += RouteChanged;
		_navigator = navigator;
	}

	private async void RouteChanged(object? sender, RouteChangedEventArgs e)
	{
		var fullRoute = ((FrameNavigator)e.Navigator).FullRoute;
		var paths = fullRoute?.Path?.Split('/').ToList();
		paths.Insert(0, fullRoute.Base);
		Breadcrumbs = new ObservableCollection<string>(paths);
	}

	public event PropertyChangedEventHandler PropertyChanged;

	protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	private readonly INavigator _navigator;

	[RelayCommand]
	public async Task NavigateToFirst()
	{
		await _navigator.NavigateViewAsync<FirstBreadcrumbPage>(this);
	}

	[RelayCommand]
	public async Task NavigateBreadcrumb()
	{
		await _navigator.NavigateRouteAsync(this, "");
	}
}
