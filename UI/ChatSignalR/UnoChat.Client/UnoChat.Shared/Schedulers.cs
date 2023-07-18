using System;
using System.Reactive.Concurrency;
using System.Threading;

namespace UnoChat.Client
{
    public static partial class Schedulers
    {
        static partial void OverrideDispatchScheduler(ref IScheduler scheduler);

        private static readonly Lazy<IScheduler> DispatcherScheduler = new Lazy<IScheduler>(
            () =>
            {
                IScheduler scheduler = null;
                OverrideDispatchScheduler(ref scheduler);
                return scheduler == null
                ? new SynchronizationContextScheduler(SynchronizationContext.Current)
                : scheduler;
            }
        );

        public static IScheduler Dispatcher => DispatcherScheduler.Value;

        public static IScheduler Default => Scheduler.Default;
    }
}
