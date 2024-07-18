namespace Navigation.Presentation;

public partial class ModalDialogViewModel
{
	private readonly INavigator _navigator;

	public ModalDialogViewModel(INavigator navigator)
	{
		_navigator = navigator;
	}

	[RelayCommand]
	private async Task ShowFlyout()
		=> await _navigator.NavigateRouteAsync(this, route: "ModalDialogSecond", qualifier: Qualifiers.Dialog);

	[RelayCommand]
	private async Task ShowContentDialog()
		=> await _navigator.NavigateRouteAsync(this, route: "ModalContentDialog", qualifier: Qualifiers.Dialog);
}
