namespace Navigation.Presentation;

public sealed partial class ModalDialogPage : Page
{
	public ModalDialogPage()
	{
		this.InitializeComponent();
	}

	private async void FlyoutRequestingDataWithCancelClick(object sender, RoutedEventArgs args)
	{
		var cancelSource = new CancellationTokenSource(TimeSpan.FromSeconds(2));
		var nav = this.Navigator()!;
		var result = await nav.NavigateRouteForResultAsync<Widget>(new object(), "!ModalDialogSecond", cancellation: cancelSource.Token).AsResult();
	}
}
