namespace Authentication.MsalExtensionsDemo.Presentation;

/// <summary>
/// Proves the access token works by calling Microsoft Graph with it.
/// </summary>
public sealed partial class GraphView : UserControl
{
    public GraphView()
    {
        InitializeComponent();
    }

    private async void OnCallGraphClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is GraphViewModel viewModel)
        {
            await viewModel.CallGraphAsync();
        }
    }
}
