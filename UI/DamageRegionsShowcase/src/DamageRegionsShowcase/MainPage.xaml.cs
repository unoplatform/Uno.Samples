using System.Collections.Generic;
using Microsoft.UI.Xaml.Media.Animation;

namespace DamageRegionsShowcase;

public record StaticCard(string Title, string Body);

public sealed partial class MainPage : Page
{
	public MainPage()
	{
		this.InitializeComponent();

		StaticCards.ItemsSource = BuildCards();
		Loaded += (_, _) => StartDotAnimation();
	}

	private static IReadOnlyList<StaticCard> BuildCards() =>
	[
		new("Order #4821", "Shipped this morning from the Munich warehouse. Two items, expedited."),
		new("Order #4822", "Awaiting payment confirmation. Reserved until Friday."),
		new("Order #4823", "Delivered and signed for. No returns window remaining."),
		new("Order #4824", "Partially fulfilled — one item on backorder until next week."),
		new("Order #4825", "Cancelled by the customer before dispatch. Refund issued."),
		new("Order #4826", "Packed and waiting for carrier pickup this afternoon."),
	];

	// A steady horizontal slide: the damage region tracks the ellipse across the canvas
	// while every card to the right of it stays untouched. Keyframes rather than
	// AutoReverse, which Uno does not implement.
	private void StartDotAnimation()
	{
		DoubleAnimationUsingKeyFrames animation = new() { EnableDependentAnimation = true, RepeatBehavior = RepeatBehavior.Forever };
		animation.KeyFrames.Add(new LinearDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero), Value = 0 });
		animation.KeyFrames.Add(new LinearDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(2)), Value = 220 });
		animation.KeyFrames.Add(new LinearDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(4)), Value = 0 });

		Storyboard storyboard = new();
		Storyboard.SetTarget(animation, DotTransform);
		Storyboard.SetTargetProperty(animation, "X");
		storyboard.Children.Add(animation);
		storyboard.Begin();
	}

	private void OnOverlayToggled(object sender, RoutedEventArgs e)
	{
		var on = OverlayToggle.IsOn;

		Uno.UI.FeatureConfiguration.Rendering.DamageRegionOverlay = on;

		StatusText.Text = on
			? "Overlay on — red marks the pixels being repainted"
			: "Overlay off — normal rendering";
	}
}
