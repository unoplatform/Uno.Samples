namespace Navigation.Presentation;

public partial class PageNavigationThreeViewModel : ObservableObject
{
	private readonly INavigator _navigator;

	public PageNavigationThreeViewModel(INavigator navigator)
	{
		_navigator = navigator;
	}

	[RelayCommand]
	public async Task GoToFour()
	{
		await _navigator.NavigateViewModelAsync<PageNavigationFourViewModel>(this);
	}

	[RelayCommand]
	public async Task GoBack()
	{
		await _navigator.GoBack(this);
	}
}