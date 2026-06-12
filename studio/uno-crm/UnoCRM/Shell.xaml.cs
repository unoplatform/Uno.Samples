using System;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Uno.Toolkit.UI;

namespace UnoCRM;

public sealed partial class Shell : UserControl
{
    // Demo-only: a fixed minimum so the splash is perceptible while the app warms up.
    // A production app would instead gate the reveal on real readiness (e.g. first
    // navigation completing / data loaded) rather than an arbitrary delay.
    private static readonly TimeSpan SplashMinimumDuration = TimeSpan.FromSeconds(2.5);

    public Shell()
    {
        this.InitializeComponent();

        var loader = new StartupLoadable();
        Splash.Source = loader;
        loader.Begin(DispatcherQueue, SplashMinimumDuration);
    }

    /// <summary>The navigation frame hosted behind the extended splash screen.</summary>
    public Frame RootFrame => ShellFrame;

    /// <summary>
    /// Minimal <see cref="ILoadable"/> that reports "executing" for a fixed delay, then
    /// flips to done so the <see cref="ExtendedSplashScreen"/> reveals its content. The
    /// flag is always cleared — even if the UI dispatcher can't be reached — so the app
    /// can never get stuck on the splash.
    /// </summary>
    private sealed class StartupLoadable : ILoadable
    {
        public bool IsExecuting { get; private set; } = true;

        public event EventHandler? IsExecutingChanged;

        public async void Begin(DispatcherQueue dispatcher, TimeSpan delay)
        {
            try
            {
                await Task.Delay(delay);
            }
            catch
            {
                // Swallow: the splash must still be dismissed below.
            }

            // Prefer the UI thread; if enqueue fails (dispatcher shutting down), complete
            // inline so IsExecuting is never left stuck true.
            if (!dispatcher.TryEnqueue(Complete))
            {
                Complete();
            }
        }

        private void Complete()
        {
            if (!IsExecuting)
            {
                return;
            }

            IsExecuting = false;
            IsExecutingChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
