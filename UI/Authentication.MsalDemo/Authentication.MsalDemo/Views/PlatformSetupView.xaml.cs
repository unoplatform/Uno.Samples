namespace Authentication.MsalDemo.Views;

/// <summary>
/// Reference material: the Entra ID registration and project changes each head needs.
/// </summary>
public sealed partial class PlatformSetupView : UserControl
{
    private readonly PlatformSetupViewModel _viewModel = new();

    public PlatformSetupView()
    {
        InitializeComponent();

        DataContext = _viewModel;
    }
}
