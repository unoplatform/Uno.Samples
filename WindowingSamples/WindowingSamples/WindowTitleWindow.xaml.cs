using Microsoft.UI.Windowing;

namespace WindowingSamples;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class WindowTitleWindow : Window
{
    public WindowTitleWindow()
    {
        InitializeComponent();
        WindowTitle = "Test Window Title";
    }

    public string WindowTitle
    {
        get => AppWindow.Title;
        set => AppWindow.Title = value;
    }
}
