namespace Navigation.Presentation;

public partial class PageNavigationFiveViewModel : ObservableObject
{

	private readonly INavigator _navigator;

	public PageNavigationFiveViewModel(INavigator navigator)
	{
		_navigator = navigator;
	}

	[RelayCommand]
	public async Task GoToSixWithData()
	{
		await _navigator.NavigateViewModelAsync<PageNavigationSixViewModel>(this, data: new Widget { Name = "Bob" });
	}

	[RelayCommand]
	public async Task GoToSixWithDataAndBack()
	{
		await _navigator.NavigateViewModelAsync<PageNavigationSixViewModel>(this, data: new Widget { Name = "Bob" }, qualifier: Qualifiers.NavigateBack);
	}

	//[RelayCommand]
	//public async Task FivePageBackCodebehindClick(object sender, RoutedEventArgs e)
	//{
	//	await _navigator.NavigateBackAsync(this);
	//}

	[RelayCommand]
	public async Task GoBack()
	{
		await _navigator.GoBack(this);
	}
}
