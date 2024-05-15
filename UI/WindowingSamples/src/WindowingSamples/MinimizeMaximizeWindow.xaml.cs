using Microsoft.UI.Windowing;

namespace WindowingSamples;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MinimizeMaximizeWindow : Window
{
    private OverlappedPresenter _presenter;

    public MinimizeMaximizeWindow()
    {
        InitializeComponent();

        _presenter = (OverlappedPresenter)AppWindow.Presenter;
    }

    public void MinimizeClick(object sender, RoutedEventArgs args) => _presenter.Minimize();

    public void RestoreClick(object sender, RoutedEventArgs args) => _presenter.Restore();

    public void MaximizeClick(object sender, RoutedEventArgs args) => _presenter.Maximize();
}
