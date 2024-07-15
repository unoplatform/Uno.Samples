namespace Navigation.Presentation;

public class PageNavigationViewModel : ObservableObject
{
	private readonly INavigator _navigator;

	public PageNavigationViewModel(INavigator navigator)
	{
		_navigator = navigator;
	}

	public ICommand NavigateCommand => new AsyncRelayCommand(NavigateToSamplePage);

	private Task NavigateToSamplePage()
	{
		return _navigator.NavigateViewAsync<SamplePage>(this);
	}

}
