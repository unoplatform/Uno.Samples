namespace Navigation.Presentation;

public class PageNavigationViewModel : ObservableObject
{
	private readonly INavigator _navigator;

	public PageNavigationViewModel(INavigator navigator)
	{
		_navigator = navigator;
		NavigateCommand = new AsyncRelayCommand(NavigateToSamplePage);
	}

	public ICommand NavigateCommand { get; }

	private Task NavigateToSamplePage()
	{
		return _navigator.NavigateViewAsync<SamplePage>(this);
	}

}
