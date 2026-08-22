using System.Collections.Specialized;
using Authentication.WebExtensionsDemo.Authentication;

namespace Authentication.WebExtensionsDemo.Presentation;

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

        // Lets the log marshal entries onto the UI thread if a callback arrives on another one.
        viewModel.Log.AttachDispatcher(DispatcherQueue);
        viewModel.Log.CollectionChanged += OnLogChanged;

        if (viewModel.Log.Count == 0)
        {
            viewModel.Log.Info(
                $"Ready on {PlatformSupport.PlatformName}",
                $"""
                Redirect URI in use: {viewModel.RedirectUri}

                Press Sign in to run the flow (demo test user: bob / bob). The provider checks
                the stored refresh token first, and only shows UI when interaction is required.
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

    private async void OnCallApiClick(object sender, RoutedEventArgs e)
    {
        if (_viewModel is { } viewModel)
        {
            await viewModel.CallApiAsync();
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
