namespace Navigation.Presentation;

public partial class SecondPageViewModel : ObservableObject
{
	private readonly INavigator _navigator;

	public SecondPageViewModel(INavigator navigator)
	{
		_navigator = navigator;
	}

	[RelayCommand]
	public async Task NavigateToThird()
	{
		await _navigator.NavigateViewAsync<ThirdPage>(this);
	}

}
