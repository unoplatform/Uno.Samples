using System;
using System.Linq;
using Microsoft.UI.Xaml.Controls.Primitives;
using Uno.UI;
using Windows.UI.ViewManagement;

namespace TextScalingShowcase;

public sealed partial class MainPage : Page
{
	private static readonly double[] LadderSizes = [12, 14, 20, 28];

	private readonly UISettings _uiSettings = new();

	public MainPage()
	{
		this.InitializeComponent();

		_uiSettings.TextScaleFactorChanged += OnOsTextScaleFactorChanged;
		UpdateOsScale();

		// Start where the OS is, so the form matches the system before anything is dragged.
		ScaleSlider.Value = Math.Clamp(_uiSettings.TextScaleFactor * 100, ScaleSlider.Minimum, ScaleSlider.Maximum);
		ScaleSlider.ValueChanged += OnScaleSliderValueChanged;

		ApplyScaleFactor(ScaleSlider.Value / 100);
	}

	private void OnScaleSliderValueChanged(object sender, RangeBaseValueChangedEventArgs e) => ApplyScaleFactor(e.NewValue / 100);

	private void OnResetClick(object sender, RoutedEventArgs e) => ScaleSlider.Value = 100;

	private void OnOsTextScaleFactorChanged(UISettings sender, object args) => DispatcherQueue.TryEnqueue(UpdateOsScale);

	private void UpdateOsScale() => OsScaleText.Text = Format(_uiSettings.TextScaleFactor);

	private void ApplyScaleFactor(double factor)
	{
		FeatureConfiguration.Font.TextScaleFactor = factor;

		ScaleValueText.Text = Format(factor);
		CurveText.Text = BuildLadder(factor);

		// The override is read when a text element builds its font, and only an OS-driven change
		// invalidates the ones already realized, so the card is rebuilt to pick up the new factor.
		FormHost.Child = new ProfileCard();
	}

	private static string Format(double factor) => $"{factor * 100:0}%  ·  factor {factor:0.00}";

	private static string BuildLadder(double factor) =>
		string.Join(
			Environment.NewLine,
			LadderSizes.Select(size => $"{size,2:0} pt  →  {TextScaleCurve.Scale(size, factor),4:0.0} pt"));
}
