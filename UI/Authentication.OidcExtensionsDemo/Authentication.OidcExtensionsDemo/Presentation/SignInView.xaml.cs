using System.Collections.Specialized;
using Authentication.OidcExtensionsDemo.Authentication;

namespace Authentication.OidcExtensionsDemo.Presentation;

/// <summary>
/// Runs the sign-in flow and narrates it. The view model arrives through the DataContext, bound
/// by <see cref="MainPage"/> from the shell model Uno.Extensions Navigation resolved.
/// </summary>
public sealed partial class SignInView : UserControl
{
    private SignInViewModel? _viewModel;

    public SignInView()
    {
        InitializeComponent();

        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        if (args.NewValue is not SignInViewModel viewModel || ReferenceEquals(viewModel, _viewModel))
        {
            return;
        }

        _viewModel = viewModel;

        // Lets the log marshal entries onto the UI thread: LoggedOut can be raised from the HTTP
        // handler's thread after an API 401, and mutating a bound collection from there would throw.
        viewModel.Log.AttachDispatcher(DispatcherQueue);
        viewModel.Log.CollectionChanged += OnLogChanged;

        if (viewModel.Log.Count == 0)
        {
            viewModel.Log.Info(
                $"Ready on {PlatformSupport.PlatformName}",
                $"""
                Redirect URI in use: {viewModel.RedirectUri}

                Press Sign in to run the authorization code flow: the provider opens the
                identity provider's page and exchanges the returned code for tokens.
                """);
        }

        _ = viewModel.RefreshTokensSummaryAsync();
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

    private async void OnSignInClick(object sender, RoutedEventArgs e)
    {
        if (_viewModel is { } viewModel)
        {
            await viewModel.SignInAsync();
        }
    }

    private async void OnRefreshClick(object sender, RoutedEventArgs e)
    {
        if (_viewModel is { } viewModel)
        {
            await viewModel.RefreshAsync();
        }
    }

    private async void OnSignOutClick(object sender, RoutedEventArgs e)
    {
        if (_viewModel is { } viewModel)
        {
            await viewModel.SignOutAsync();
        }
    }

    private void OnClearLogClick(object sender, RoutedEventArgs e) => _viewModel?.ClearLog();
}
