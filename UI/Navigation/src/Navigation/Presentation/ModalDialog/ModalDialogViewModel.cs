namespace Navigation.Presentation;

public class ModalDialogViewModel
{
	private readonly INavigator _navigator;

	public ModalDialogViewModel(INavigator navigator)
	{
		_navigator = navigator;
	}

	public ICommand ShowFlyoutCommand => new AsyncRelayCommand(ShowFlyout);

	public ICommand ShowContentDialogCommand => new AsyncRelayCommand(ShowContentDialog);

	private async Task ShowFlyout()
	{
		await _navigator.NavigateRouteAsync(this, route: "ModalDialogSecond", qualifier: Qualifiers.Dialog);
	}

	private async Task ShowContentDialog()
	{
		await _navigator.NavigateRouteAsync(this, route: "ModalContentDialog", qualifier: Qualifiers.Dialog);
	}
}
