using System.Collections.Specialized;
using Authentication.OidcSteve.AuthFlow;
using Microsoft.Extensions.DependencyInjection;

namespace Authentication.OidcSteve.Presentation;

public sealed partial class MainPage : Page
{
    private readonly AuthFlowService _flow;

    public MainPage()
    {
        this.InitializeComponent();

        // The narrated flow lives in a singleton so it survives page navigation; the log and the
        // token summary are wired here rather than through the model to keep the model thin.
        _flow = ((App)Application.Current).Services!.GetRequiredService<AuthFlowService>();

        _flow.Log.AttachDispatcher(DispatcherQueue);
        LogItems.ItemsSource = _flow.Log;
        _flow.Log.CollectionChanged += OnLogChanged;
        _flow.StateChanged += OnFlowStateChanged;

        PlatformChip.Text = PlatformSupport.PlatformName;

        Loaded += (_, _) => _ = RefreshSummariesAsync();
    }

    private void OnFlowStateChanged(object? sender, EventArgs e)
    {
        if (DispatcherQueue.HasThreadAccess)
        {
            _ = RefreshSummariesAsync();
        }
        else
        {
            DispatcherQueue.TryEnqueue(() => _ = RefreshSummariesAsync());
        }
    }

    private async Task RefreshSummariesAsync()
    {
        try
        {
            TokensSummaryText.Text = await _flow.DescribeTokensAsync();

            var response = _flow.ApiResponse;
            ApiResponseText.Text = response ?? "";
            ApiCard.Visibility = string.IsNullOrEmpty(response) ? Visibility.Collapsed : Visibility.Visible;
        }
        catch (Exception)
        {
            // Never let a summary refresh take the page down; the flow log carries the real errors.
        }
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
}
