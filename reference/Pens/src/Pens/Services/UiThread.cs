using Microsoft.UI.Dispatching;

namespace Pens.Services;

/// <summary>
/// Marshals bound-state updates to the UI thread. ViewModels are constructed on
/// a background thread by the navigation system, so the UI DispatcherQueue is
/// captured once at app startup (on the UI thread) and reused.
/// </summary>
public static class UiThread
{
    public static DispatcherQueue? Dispatcher { get; set; }

    public static void Run(Action action)
    {
        var dispatcher = Dispatcher;
        if (dispatcher is null || dispatcher.HasThreadAccess)
            action();
        else
            dispatcher.TryEnqueue(() => action());
    }
}
