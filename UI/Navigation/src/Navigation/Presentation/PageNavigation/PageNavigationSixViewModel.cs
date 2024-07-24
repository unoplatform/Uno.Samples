namespace Navigation.Presentation;

public partial class PageNavigationSixViewModel : ObservableObject
{
	private readonly INavigator _navigator;

	public PageNavigationSixViewModel(INavigator navigator)
	{
		_navigator = navigator;
	}

	[RelayCommand]
	public async Task GoToSeven()
	{
		await _navigator.NavigateViewModelAsync<PageNavigationSevenViewModel>(this);
	}
}