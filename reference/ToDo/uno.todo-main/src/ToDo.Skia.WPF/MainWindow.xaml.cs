using Window = System.Windows.Window;

namespace ToDo.WPF;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();

		root.Content = new global::Uno.UI.Skia.Platform.WpfHost(Dispatcher, () => new ToDo.App());
	}
}
