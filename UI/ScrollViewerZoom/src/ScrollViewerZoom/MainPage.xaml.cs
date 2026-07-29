using System.Globalization;

namespace ScrollViewerZoom;

public sealed partial class MainPage : Page
{
	public MainPage()
	{
		this.InitializeComponent();
	}

	private void OnScrollViewerLoaded(object sender, RoutedEventArgs e) => UpdateZoomReadout();

	private void OnViewChanged(object sender, ScrollViewerViewChangedEventArgs e) => UpdateZoomReadout();

	private void OnZoomToPreset(object sender, RoutedEventArgs e)
	{
		var factor = float.Parse((string)((Button)sender).Tag, CultureInfo.InvariantCulture);
		PlanScrollViewer.ChangeView(null, null, factor);
		FocusPlan();
	}

	private void OnZoomToFit(object sender, RoutedEventArgs e)
	{
		PlanScrollViewer.ChangeView(0, 0, GetFitZoomFactor());
		FocusPlan();
	}

	private float GetFitZoomFactor()
	{
		var viewportWidth = PlanScrollViewer.ViewportWidth;
		var viewportHeight = PlanScrollViewer.ViewportHeight;
		if (viewportWidth <= 0 || viewportHeight <= 0)
		{
			return 1f;
		}

		var fit = Math.Min(viewportWidth / FloorPlanCanvas.Width, viewportHeight / FloorPlanCanvas.Height);
		return (float)Math.Clamp(fit, PlanScrollViewer.MinZoomFactor, PlanScrollViewer.MaxZoomFactor);
	}

	// ScrollViewer.OnKeyDown handles Ctrl+Plus / Ctrl+Minus, so hand focus back to the plan
	// instead of leaving it on the button that was just clicked.
	private void FocusPlan() => _ = PlanScrollViewer.Focus(FocusState.Programmatic);

	private void UpdateZoomReadout()
		=> ZoomFactorText.Text = PlanScrollViewer.ZoomFactor.ToString("0.00", CultureInfo.InvariantCulture) + "x";
}
