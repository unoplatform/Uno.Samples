namespace PhotoViewer;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
    }

    private void ResetZoom(object sender, RoutedEventArgs e)
    {
        mContent.ResetZoom();
    }

    private void ResetOffset(object sender, RoutedEventArgs e)
    {
        mContent.ResetOffset();
    }
}
