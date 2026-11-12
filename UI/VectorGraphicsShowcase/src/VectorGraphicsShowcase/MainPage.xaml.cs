using Microsoft.UI.Xaml.Media.Animation;

namespace VectorGraphicsShowcase;

public sealed partial class MainPage : Page
{
	private Storyboard? _spin;

	public MainPage()
	{
		this.InitializeComponent();
		Loaded += OnLoaded;
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		var anim = new DoubleAnimation
		{
			From = 0,
			To = 360,
			Duration = new Duration(System.TimeSpan.FromSeconds(4)),
			RepeatBehavior = RepeatBehavior.Forever,
			EnableDependentAnimation = true,
		};
		Storyboard.SetTarget(anim, SpinTransform);
		Storyboard.SetTargetProperty(anim, "Angle");
		_spin = new Storyboard();
		_spin.Children.Add(anim);
		_spin.Begin();
	}

	private void OnSpinToggled(object sender, RoutedEventArgs e)
	{
		if (_spin is null)
		{
			return;
		}

		if (SpinToggle.IsChecked == true)
		{
			_spin.Resume();
		}
		else
		{
			_spin.Pause();
		}
	}
}
