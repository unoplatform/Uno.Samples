using System.Numerics;
using Windows.UI;
using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Media.Animation;

namespace VectorGraphicsShowcase;

public sealed partial class MainPage : Page
{
	private Storyboard? _geometrySpin;

	public MainPage()
	{
		this.InitializeComponent();
		this.Loaded += OnLoaded;
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		SetupAlphaMasks();
		SetupGeometryAnimation();
	}

	private void SetupAlphaMasks()
	{
		var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

		var gradient = compositor.CreateLinearGradientBrush();
		gradient.ColorStops.Add(compositor.CreateColorGradientStop(0.0f, Colors.HotPink));
		gradient.ColorStops.Add(compositor.CreateColorGradientStop(1.0f, Colors.DeepSkyBlue));
		gradient.StartPoint = new Vector2(0, 0);
		gradient.EndPoint = new Vector2(1, 1);

		ApplyMask(compositor, MaskSourceEllipse.GetAlphaMask(), gradient, MaskResultCanvas, new Vector2(140, 140));

		var textGradient = compositor.CreateLinearGradientBrush();
		textGradient.ColorStops.Add(compositor.CreateColorGradientStop(0.0f, Colors.Orange));
		textGradient.ColorStops.Add(compositor.CreateColorGradientStop(1.0f, Colors.MediumPurple));
		textGradient.StartPoint = new Vector2(0, 0);
		textGradient.EndPoint = new Vector2(1, 0);

		ApplyMask(compositor, MaskSourceText.GetAlphaMask(), textGradient, MaskTextResultCanvas, new Vector2(180, 60));
	}

	private static void ApplyMask(Compositor compositor, CompositionBrush mask, CompositionBrush source, Canvas host, Vector2 size)
	{
		var maskBrush = compositor.CreateMaskBrush();
		maskBrush.Source = source;
		maskBrush.Mask = mask;

		var visual = compositor.CreateSpriteVisual();
		visual.Brush = maskBrush;
		visual.Size = size;

		ElementCompositionPreview.SetElementChildVisual(host, visual);
	}

	private void SetupGeometryAnimation()
	{
		var animation = new DoubleAnimation
		{
			From = 0,
			To = 360,
			Duration = new Duration(TimeSpan.FromSeconds(4)),
			RepeatBehavior = RepeatBehavior.Forever
		};

		Storyboard.SetTarget(animation, GeometrySpin);
		Storyboard.SetTargetProperty(animation, nameof(CompositeTransform.Rotation));

		_geometrySpin = new Storyboard();
		_geometrySpin.Children.Add(animation);

		if (AnimateToggle.IsChecked == true)
		{
			_geometrySpin.Begin();
		}
	}

	private void OnAnimateToggled(object sender, RoutedEventArgs e)
	{
		if (_geometrySpin is null)
		{
			return;
		}

		if (AnimateToggle.IsChecked == true)
		{
			_geometrySpin.Begin();
		}
		else
		{
			_geometrySpin.Stop();
		}
	}
}
