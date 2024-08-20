namespace Navigation.Presentation;

public partial class ComplexFlyoutOneViewModel
{
	private INavigator Navigator { get; }

	public ICommand CloseCommand { get; }

	public string? Name { get; set; }

	public DialogsFlyoutsData[] Items => new[]
	{
		new DialogsFlyoutsData(),
		new DialogsFlyoutsData(),
		new DialogsFlyoutsData(),
		new DialogsFlyoutsData()
	};

	public ComplexFlyoutOneViewModel(INavigator navigator)
	{

		Navigator = navigator;

		CloseCommand = new AsyncRelayCommand(Close);
	}

	public async Task Close()
	{
		await Navigator.NavigateBackAsync(this, qualifier: Qualifiers.Root);
	}
}
