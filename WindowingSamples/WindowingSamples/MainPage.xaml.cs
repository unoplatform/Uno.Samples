using Microsoft.UI;

namespace WindowingSamples;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
    }

    public void BasicWindowSample()
    {
        var window = new Window();
        var textBlock = new TextBlock() { Text = "Hello, from second window!", FontSize = 60 };
        window.Content = textBlock;
        window.Activate();
    }

    public void CustomWindowSample() { } // => new CustomWindow().Activate();

    public void FullScreenModeSample() { } // => new FullScreenModeWindow().Activate();

    public void MinimizeMaximizeSample() { } // => new MinimizeMaximizeWindow().Activate();

    public void WindowTitleSample() { } // => new WindowTitleWindow().Activate();

    public void StayOnTopSample() => new StayOnTopWindow().Activate();
}
