namespace Navigation.Presentation;

public partial class ThirdPageViewModel : ObservableObject
{
	private readonly INavigator _navigator;

	public ThirdPageViewModel(INavigator navigator)
	{
		_navigator = navigator;
	}

	[RelayCommand]
	public async Task NavigateToThird()
	{
		await _navigator.NavigateViewAsync<ThirdPage>(this);
	}

}
