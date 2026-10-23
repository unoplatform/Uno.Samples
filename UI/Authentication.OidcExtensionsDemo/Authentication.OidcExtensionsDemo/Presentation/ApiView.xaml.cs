namespace Authentication.OidcExtensionsDemo.Presentation;

/// <summary>
/// Proves the access token works by calling the demo server's protected API with it.
/// </summary>
public sealed partial class ApiView : UserControl
{
    public ApiView()
    {
        InitializeComponent();
    }

    private async void OnCallApiClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is ApiViewModel viewModel)
        {
            await viewModel.CallApiAsync();
        }
    }
}
