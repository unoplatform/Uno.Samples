namespace Navigation.Presentation;

public partial class PageNavigationViewModel : ObservableObject
{
	private readonly INavigator _navigator;

	public PageNavigationViewModel(INavigator navigator)
	{
		_navigator = navigator;
	}

	[RelayCommand]
	public async Task Navigate()
	{
		await _navigator.NavigateViewAsync<SamplePage>(this);
	}
}
