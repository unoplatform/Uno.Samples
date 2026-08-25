using System.Collections.Specialized;
using Authentication.WebExtensionsDemo.AuthFlow;
using Microsoft.Extensions.DependencyInjection;

namespace Authentication.WebExtensionsDemo.Presentation;

public sealed partial class LoginPage : Page
{
    public LoginPage()
    {
        this.InitializeComponent();

        // The narrated flow lives in a singleton so it survives page navigation; the log and the
        // configuration card are wired here rather than through the model to keep the model thin.
        var flow = ((App)Application.Current).Services!.GetRequiredService<AuthFlowService>();

        flow.Log.AttachDispatcher(DispatcherQueue);
        LogItems.ItemsSource = flow.Log;
        flow.Log.CollectionChanged += OnLogChanged;

        PlatformChip.Text = PlatformSupport.PlatformName;
        LoginStartText.Text = flow.LoginStartUri;
        TokenKeysText.Text = flow.TokenKeys;
        RedirectText.Text = flow.LoginCallbackUri;
        SurfaceText.Text = PlatformSupport.SignInSurface;
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
