namespace Navigation.Presentation;

public partial class MessageDialogViewModel
{
	private readonly INavigator _navigator;

	public MessageDialogViewModel(INavigator navigator)
	{
		_navigator = navigator;
	}

	[RelayCommand]
	private async Task ShowSimpleDialog()
	{
		var result = await _navigator.ShowMessageDialogAsync<string>(this,
			title: "This is Uno",
			content: "Hello Uno.Extensions!",
			route: "MyMessage",
			buttons:
			[
				new DialogAction("Ok"),
				new DialogAction("Cancel")
			]);
	}
}
