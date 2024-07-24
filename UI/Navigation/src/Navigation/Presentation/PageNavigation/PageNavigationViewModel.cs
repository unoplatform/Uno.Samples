namespace Navigation.Presentation;

public partial class PageNavigationViewModel : ObservableObject
{
	private readonly INavigator _navigator;

	public PageNavigationViewModel(INavigator navigator)
	{
		_navigator = navigator;
	}

	[RelayCommand]
	public async Task GoToTwo()
	{
		await _navigator.NavigateViewModelAsync<PageNavigationTwoViewModel>(this);
	}

}
