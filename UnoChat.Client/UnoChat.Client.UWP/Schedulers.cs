using System.Reactive.Concurrency;
using Windows.UI.Xaml;

namespace UnoChat.Client
{
    public static partial class Schedulers
    {
        static partial void OverrideDispatchScheduler(ref IScheduler scheduler)
        {
            scheduler = new CoreDispatcherScheduler(Window.Current.Dispatcher);
        }
    }
}
