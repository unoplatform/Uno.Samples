namespace DenimOverallsApp.Presentation;

public sealed partial class MainPage : Page
{
    // Preview keeps this width whenever the layout is wide enough; below that it yields
    // space so the options column never drops under its minimum (which would clip the cards).
    private const double PreferredPreviewWidth = 300;
    private const double MinOptionsWidth = 220;

    public MainPage()
    {
        this.InitializeComponent();
        // DataContext is provided by navigation (the initial "Main" route resolves MainModel from DI).
    }

    private void OnBodySizeChanged(object sender, SizeChangedEventArgs e)
    {
        // Drive the split from the Grid's real width, not the window width: in the studio's
        // mobile frame the window is wide while the rendered frame is narrow, which is what
        // made the original AdaptiveTrigger pick the wide layout and clip the options column.
        var available = e.NewSize.Width;
        var preview = Math.Max(0, Math.Min(PreferredPreviewWidth, available - MinOptionsWidth));
        PreviewCol.Width = new GridLength(preview);
    }
}
