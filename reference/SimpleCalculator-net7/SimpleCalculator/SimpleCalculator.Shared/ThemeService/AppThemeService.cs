using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Uno.Toolkit.UI;
using Microsoft.UI.Xaml;

namespace SimpleCalculator.ThemeService
{
    public class AppThemeService : IAppThemeService
    {
        private readonly Window _window;

        public AppThemeService()
        {
            //wip - doenst work on windows
            if(Window.Current != null)
            {
                _window = Window.Current;
            }
        }

        public bool IsDark => SystemThemeHelper.IsRootInDarkMode(_window.Content.XamlRoot!);

        public async ValueTask SetThemeAsync(bool darkMode, CancellationToken ct)
        {
            var tcs = new TaskCompletionSource<object?>();
            await using var _ = ct.Register(() => tcs.TrySetCanceled());
            _window.DispatcherQueue.TryEnqueue(() =>
            {
                if (!ct.IsCancellationRequested)
                {
                    SystemThemeHelper.SetRootTheme(_window.Content.XamlRoot, darkMode);
                }

                tcs.TrySetResult(default);
            });

            await tcs.Task;
        }
    }
}
