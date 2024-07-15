namespace Navigation.Presentation;

public class MessageDialogViewModel
{
	private readonly INavigator _navigator;
	public MessageDialogViewModel(INavigator navigator)
	{
		_navigator = navigator;
		ShowSimpleDialogCommand = new AsyncRelayCommand(ShowSimpleDialog);
	}
	public ICommand ShowSimpleDialogCommand { get; }
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
