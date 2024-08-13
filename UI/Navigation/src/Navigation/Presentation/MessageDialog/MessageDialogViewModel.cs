namespace Navigation.Presentation;

public partial class MessageDialogViewModel : ObservableObject
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

	[RelayCommand]
	private async Task MessageDialogCodebehindCancel()
	{
		var cancelSource = new CancellationTokenSource(TimeSpan.FromSeconds(2));
		var messageDialogResult = await _navigator.ShowMessageDialogAsync<string>(this, content: "This is Content", title: "This is title", cancellation: cancelSource.Token);
		MessageDialogResultText = $"Message dialog result: {messageDialogResult}";
	}

	[ObservableProperty]
	private string messageDialogResultText;
}
