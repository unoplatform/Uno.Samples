using Uno.Toolkit.UI;

namespace SimpleCalculator.Views;

public sealed partial class Shell : UserControl
{
	public ExtendedSplashScreen SplashScreen => Splash;

	public Shell()
	{
		this.InitializeComponent();
	}
}
