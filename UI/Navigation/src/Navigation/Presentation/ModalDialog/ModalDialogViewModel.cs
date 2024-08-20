namespace Navigation.Presentation;

public partial class ModalDialogViewModel : ObservableObject
{
	private readonly INavigator _navigator;

	public ModalDialogViewModel(INavigator navigator)
	{
		_navigator = navigator;
		flyoutData = new DialogsFlyoutsData();
	}

	[RelayCommand]
	private async Task ShowFlyout()
		=> await _navigator.NavigateRouteAsync(this, route: "ModalDialogSecond", qualifier: Qualifiers.Dialog);

	[RelayCommand]
	private async Task ShowContentDialog()
		=> await _navigator.NavigateRouteAsync(this, route: "ModalContentDialog", qualifier: Qualifiers.Dialog);

	[ObservableProperty]
	private DialogsFlyoutsData flyoutData;

	[RelayCommand]
	private async Task FlyoutRequestingDataWithCancel()
	{
		var cancelSource = new CancellationTokenSource(TimeSpan.FromSeconds(2));
		var result = await _navigator.NavigateRouteForResultAsync<Widget>(new object(), "!ModalDialogSecond", cancellation: cancelSource.Token).AsResult();
	}
}

public class DialogsFlyoutsData
{
	public Guid Id { get; } = Guid.NewGuid();
}
