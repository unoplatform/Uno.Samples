using Microsoft.UI.Windowing;

namespace WindowingSamples;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class FullScreenModeWindow : Window
{
    public FullScreenModeWindow()
    {
        InitializeComponent();

        // Set the window to full screen mode
        AppWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
    }

    public void CloseClick(object sender, RoutedEventArgs args) => Close();
}
