namespace Authentication.MsalDemo.Views;

/// <summary>
/// Proves the access token works by calling Microsoft Graph.
/// </summary>
public sealed partial class GraphView : UserControl
{
    private readonly GraphViewModel _viewModel = new();

    public GraphView()
    {
        InitializeComponent();

        DataContext = _viewModel;
    }

    private async void OnCallGraphClick(object sender, RoutedEventArgs e) => await _viewModel.CallGraphAsync();
}
