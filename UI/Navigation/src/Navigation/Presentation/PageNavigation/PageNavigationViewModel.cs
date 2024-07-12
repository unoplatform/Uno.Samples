namespace Navigation.Presentation;

public class PageNavigationViewModel : ObservableObject
{
	private readonly INavigator _navigator;

	public PageNavigationViewModel(INavigator navigator)
	{
		_navigator = navigator;
		NavigateCommand = new AsyncRelayCommand(NavigateToSamplePage);
		ShowSimpleDialogCommand = new AsyncRelayCommand(ShowSimpleDialog);
	}

	public ICommand NavigateCommand { get; }
	public ICommand ShowSimpleDialogCommand { get; }

	private Task NavigateToSamplePage()
	{
		return _navigator.NavigateViewAsync<SamplePage>(this);
	}

	private async Task ShowSimpleDialog()
	{
		var result = await _navigator.ShowMessageDialogAsync<string>(this,
			title: "This is Uno",
			content: "Hello Uno.Extensions!",
			route: "MyMessage",
			buttons: new[]
			{
				new DialogAction("Ok"),
				new DialogAction("Cancel")
			});

	}

}
