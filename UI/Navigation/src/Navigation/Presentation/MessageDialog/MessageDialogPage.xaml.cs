namespace Navigation.Presentation;

public sealed partial class MessageDialogPage : Page
{
	public MessageDialogPage()
	{
		this.InitializeComponent();
	}

	private async void MessageDialogCodebehindRouteOverrideClick(object sender, RoutedEventArgs args)
	{
		var messageDialogResult = await this.Navigator()!.ShowMessageDialogAsync<string>(this, route: "LocalizedConfirm", content: "Override content", title: "Override title");
		MessageDialogResultText.Text = $"Message dialog result: {messageDialogResult}";
	}

	private async void MessageDialogCodebehindCancelClick(object sender, RoutedEventArgs args)
	{
		var cancelSource = new CancellationTokenSource(TimeSpan.FromSeconds(2));
		var messageDialogResult = await this.Navigator()!.ShowMessageDialogAsync<string>(this, content: "This is Content", title: "This is title", cancellation: cancelSource.Token);
		MessageDialogResultText.Text = $"Message dialog result: {messageDialogResult}";
	}
}
