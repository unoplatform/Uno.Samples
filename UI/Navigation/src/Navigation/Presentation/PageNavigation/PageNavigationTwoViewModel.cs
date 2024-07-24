namespace Navigation.Presentation;

public partial class PageNavigationTwoViewModel : ObservableObject
{
	private readonly INavigator _navigator;

	public PageNavigationTwoViewModel(INavigator navigator)
	{
		_navigator = navigator;
	}

	[RelayCommand]
	public async Task GoToThree()
	{
		await _navigator.NavigateViewModelAsync<PageNavigationThreeViewModel>(this);
	}

	[RelayCommand]
	public async Task GoBack()
	{
		await _navigator.GoBack(this);
	}
}