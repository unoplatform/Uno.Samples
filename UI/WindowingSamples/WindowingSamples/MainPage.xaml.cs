using Microsoft.UI;

namespace WindowingSamples;

public sealed partial class MainPage : Page
{
    public MainPage() => InitializeComponent();

    public void BasicWindowSample()
    {
        // Create window instance
        var window = new Window();

        // Set up content
        var grid = new Grid()
        {
            Background = new SolidColorBrush(Colors.CornflowerBlue),
            Children =
            {
                new TextBlock()
                {
                    Text = "Hello from second window!",
                    Foreground = new SolidColorBrush(Colors.White),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 60
                }
            }
        };

        window.Content = grid;

        // Show window
        window.Activate();
    }

    public void CustomWindowSample() => new CustomWindow().Activate();

    public void FullScreenModeSample() => new FullScreenModeWindow().Activate();

    public void MinimizeMaximizeSample() => new MinimizeMaximizeWindow().Activate();

    public void WindowTitleSample() => new WindowTitleWindow().Activate();

    public void StayOnTopSample() => new StayOnTopWindow().Activate();
}
