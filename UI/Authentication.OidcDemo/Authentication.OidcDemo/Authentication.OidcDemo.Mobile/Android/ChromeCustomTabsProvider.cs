using AndroidX.Browser.CustomTabs;
using System;
using System.Threading;
using System.Threading.Tasks;
using Uno.AuthenticationBroker;
using Uno.UI;
using Windows.Security.Authentication.Web;
namespace Authentication.OidcDemo.Droid
{
	public class ChromeCustomTabsProvider : WebAuthenticationBrokerProvider
	{
		protected override Task<WebAuthenticationResult> AuthenticateAsyncCore(WebAuthenticationOptions options, Uri requestUri, Uri callbackUri, CancellationToken ct)
		{
            var builder = new CustomTabsIntent.Builder();
            var intent = builder.Build();

            intent.LaunchUrl(
                ContextHelper.Current,
                Android.Net.Uri.Parse(requestUri.OriginalString));

            return base.AuthenticateAsyncCore(options, requestUri, callbackUri, ct);
		}		
	}
}