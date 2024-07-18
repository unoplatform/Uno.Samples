namespace Navigation.Presentation;

public partial class PageNavigationViewModel
{
	private readonly INavigator _navigator;

	public PageNavigationViewModel(INavigator navigator)
	{
		_navigator = navigator;
	}

	[RelayCommand]
	private async Task NavigateToSample()
		=> await _navigator.NavigateViewAsync<SamplePage>(this);

}
