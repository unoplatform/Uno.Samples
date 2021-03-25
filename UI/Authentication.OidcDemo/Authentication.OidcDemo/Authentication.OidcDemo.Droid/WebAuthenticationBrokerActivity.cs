using Android.App;
using Android.Content.PM;
using Uno.AuthenticationBroker;

namespace Authentication.OidcDemo.Droid
{
	[Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop)]
	[IntentFilter(
		new[] { Android.Content.Intent.ActionView },
		Categories = new[] { Android.Content.Intent.CategoryDefault, Android.Content.Intent.CategoryBrowsable },
		// Change the following DataScheme for one specific to your application...
		DataScheme = "uno-wab-demo")]
	public partial class WebAuthenticationBrokerActivity : Uno.AuthenticationBroker.WebAuthenticationBrokerActivityBase
	{
		// You can change the name of this class if you wish, it really not something important.
	}
}