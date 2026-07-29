using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml.Media;

namespace SystemBackdropShowcase;

public sealed partial class MainPage : Page
{
	public MainPage()
	{
		this.InitializeComponent();

		bool micaSupported = MicaController.IsSupported();
		bool acrylicSupported = DesktopAcrylicController.IsSupported();

		MicaSupportText.Text = $"MicaController.IsSupported()\n    -> {micaSupported}";
		AcrylicSupportText.Text = $"DesktopAcrylicController.IsSupported()\n    -> {acrylicSupported}";
		SupportNoticeText.Visibility = micaSupported && acrylicSupported
			? Visibility.Collapsed
			: Visibility.Visible;

		BackdropSelector.SelectedIndex = 1;
		ApplyBackdrop();

		Loaded += OnLoaded;
	}

	// The window can only make its content transparent once that content is in the visual tree.
	private void OnLoaded(object sender, RoutedEventArgs e) => ApplyBackdrop();

	private void OnBackdropSelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyBackdrop();

	private void ApplyBackdrop()
	{
		if (App.MainWindow is not { } window)
		{
			return;
		}

		SystemBackdrop? backdrop = BackdropSelector.SelectedIndex switch
		{
			1 => new MicaBackdrop(),
			2 => new DesktopAcrylicBackdrop(),
			_ => null,
		};

		window.SystemBackdrop = backdrop;
	}
}
