using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using AndroidX.Browser.CustomTabs;
using Uno.AuthenticationBroker;
using Uno.UI;

namespace Authentication.OidcDemo.Droid
{
	public class ChromeCustomTabsProvider : WebAuthenticationBrokerProvider
	{
		protected override async Task LaunchBrowserCore(
			WebAuthenticationOptions options,
			Uri requestUri,
			Uri callbackUri,
			CancellationToken ct)
		{
			var builder = new CustomTabsIntent.Builder();
			var intent = builder.Build();
			
			intent.LaunchUrl(
				ContextHelper.Current,
				Android.Net.Uri.Parse(requestUri.OriginalString));
		}
	}
}