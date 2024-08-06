namespace Navigation.Presentation;

public partial class FirstPageViewModel : ObservableObject
{
	private readonly INavigator _navigator;

	public FirstPageViewModel(INavigator navigator)
	{
		_navigator = navigator;
	}

	[RelayCommand]
	public async Task NavigateToSecond()
	{
		await _navigator.NavigateViewAsync<SecondPage>(this);
	}

}
