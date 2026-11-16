using System.Collections.Specialized;
using Authentication.MsalDemo.Authentication;

namespace Authentication.MsalDemo.Views;

/// <summary>
/// Runs the sign-in flow and narrates it.
/// </summary>
public sealed partial class SignInView : UserControl
{
    private readonly SignInViewModel _viewModel = new();

    public SignInView()
    {
        InitializeComponent();

        DataContext = _viewModel;

        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        // Lets the log marshal entries onto the UI thread if MSAL calls back from another one.
        _viewModel.Log.AttachDispatcher(DispatcherQueue);
        _viewModel.Log.CollectionChanged += OnLogChanged;

        if (_viewModel.Log.Count == 0)
        {
            _viewModel.Log.Info(
                $"Ready on {PlatformSupport.PlatformName}",
                $"""
                Redirect URI in use: {PlatformSupport.RedirectUri}

                Press Sign in to run the flow a production app should use: check the token cache
                first, and only show UI when MSAL reports that interaction is required.
                """);
        }

        await _viewModel.RefreshAccountsAsync();
    }

    private void OnLogChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action != NotifyCollectionChangedAction.Add)
        {
            return;
        }

        // Keep the newest step in view. Queued so it runs after the item has been laid out.
        DispatcherQueue.TryEnqueue(() =>
            RootScroll.ChangeView(null, RootScroll.ScrollableHeight, null, disableAnimation: false));
    }

    private async void OnSignInClick(object sender, RoutedEventArgs e) => await _viewModel.SignInAsync();

    private async void OnSilentClick(object sender, RoutedEventArgs e) => await _viewModel.SignInSilentlyAsync();

    private async void OnSignOutClick(object sender, RoutedEventArgs e) => await _viewModel.SignOutAsync();

    private void OnClearLogClick(object sender, RoutedEventArgs e) => _viewModel.ClearLog();
}
