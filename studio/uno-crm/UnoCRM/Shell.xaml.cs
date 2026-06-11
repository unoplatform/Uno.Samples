using System;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Uno.Toolkit.UI;

namespace UnoCRM;

public sealed partial class Shell : UserControl
{
    public Shell()
    {
        this.InitializeComponent();

        // Keep the extended splash up briefly so it's perceptible while the app warms up.
        var loader = new StartupLoadable();
        Splash.Source = loader;
        loader.Begin(DispatcherQueue, TimeSpan.FromSeconds(2.5));
    }

    /// <summary>The navigation frame hosted behind the extended splash screen.</summary>
    public Frame RootFrame => ShellFrame;

    /// <summary>
    /// Minimal <see cref="ILoadable"/> that reports "executing" for a fixed delay, then
    /// flips to done on the UI thread so the <see cref="ExtendedSplashScreen"/> reveals its content.
    /// </summary>
    private sealed class StartupLoadable : ILoadable
    {
        public bool IsExecuting { get; private set; } = true;

        public event EventHandler? IsExecutingChanged;

        public void Begin(DispatcherQueue dispatcher, TimeSpan delay)
        {
            _ = Task.Delay(delay).ContinueWith(_ =>
                dispatcher.TryEnqueue(() =>
                {
                    IsExecuting = false;
                    IsExecutingChanged?.Invoke(this, EventArgs.Empty);
                }));
        }
    }
}
