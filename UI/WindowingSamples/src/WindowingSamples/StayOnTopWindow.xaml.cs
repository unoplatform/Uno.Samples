using Microsoft.UI.Windowing;

namespace WindowingSamples;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class StayOnTopWindow : Window
{
    public StayOnTopWindow()
    {
        this.InitializeComponent();

        var presenter = (OverlappedPresenter)AppWindow.Presenter;
        presenter.IsAlwaysOnTop = true;
    }
}
