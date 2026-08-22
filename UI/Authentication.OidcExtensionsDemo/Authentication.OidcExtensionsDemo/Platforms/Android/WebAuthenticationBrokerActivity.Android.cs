using Android.App;
using Android.Content;
using Android.Content.PM;

namespace Authentication.OidcExtensionsDemo.Droid;

/// <summary>
/// Receives the identity provider's redirect on Android: Uno's WebAuthenticationBroker derives
/// the app's callback URI (oidc-ext-demo:///) from this activity's intent filter, and the custom
/// tab returns here when the provider redirects to it.
/// </summary>
[Activity(NoHistory = true, Exported = true, LaunchMode = LaunchMode.SingleTop)]
[IntentFilter(
    new[] { Intent.ActionView },
    Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
    DataScheme = "oidc-ext-demo")]
public partial class WebAuthenticationBrokerActivity : Uno.AuthenticationBroker.WebAuthenticationBrokerActivityBase
{
}
