namespace Navigation.Presentation;

public partial class PageNavigationFourViewModel : ObservableObject
{

	private readonly INavigator _navigator;

	public PageNavigationFourViewModel(INavigator navigator)
	{
		_navigator = navigator;
	}

	[RelayCommand]
	public async Task GoToFive()
	{
		await _navigator.NavigateViewModelAsync<PageNavigationFiveViewModel>(this);
	}

	[RelayCommand]
	public async Task GoBack()
	{
		await _navigator.GoBack(this);
	}
}
