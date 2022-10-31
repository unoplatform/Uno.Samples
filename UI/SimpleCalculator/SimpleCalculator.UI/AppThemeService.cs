using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.UI.Xaml;
using Uno.Extensions;
using Uno.Toolkit.UI;

namespace SimpleCalculator
{
	public class AppThemeService : IAppThemeService
	{
		private readonly Window _window;

		public AppThemeService(Window window)
		{
			_window = window;
		}

		public bool IsDark => SystemThemeHelper.IsRootInDarkMode(_window.Content.XamlRoot!);

		public async ValueTask SetThemeAsync(bool darkMode, CancellationToken ct)
		{
			var tcs = new TaskCompletionSource<object>();
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
