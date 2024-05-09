using Microsoft.UI.Windowing;
using Windows.UI;

namespace WindowingSamples;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class CustomWindow : Window
{
    private static int _customWindowCounter = 0;

    public CustomWindow()
    {
        InitializeComponent();

        var random = Random.Shared;
        RootGrid.Background = new SolidColorBrush(
            Color.FromArgb(
                255, 
                (byte)random.Next(0,150), 
                (byte)random.Next(0, 150), 
                (byte)random.Next(0,150)));

        var text = $"Custom Window {++_customWindowCounter}";
        Title = text;
        WindowText.Text = text;
    }

    public void CloseClick(object sender, RoutedEventArgs args) => Close();
}
