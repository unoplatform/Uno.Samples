using Uno.UI.Runtime.Skia.Wpf;
using Window = System.Windows.Window;

namespace ToDo.WPF;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();

		root.Content = new WpfHost(Dispatcher, () => new ToDo.App());
	}
}
