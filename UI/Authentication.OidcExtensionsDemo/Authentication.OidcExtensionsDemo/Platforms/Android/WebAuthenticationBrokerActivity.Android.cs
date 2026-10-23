using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Android.Views;

namespace Authentication.OidcExtensionsDemo.Droid;

/// <summary>
/// Catches the redirect back from the Chrome custom tab after sign-in and sign-out.
/// </summary>
/// <remarks>
/// Uno's <c>WebAuthenticationBroker</c> finds this activity by its base class and reads the
/// scheme off the intent filter; the configured <c>RedirectUri</c> must use the same scheme, or
/// the browser has nowhere to deliver the callback and <c>LoginAsync</c> never completes. The
/// template generates the placeholder <c>myprotocol</c>; intent filters are compile-time
/// attributes, so the scheme cannot follow appsettings automatically.
/// </remarks>
[Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
[IntentFilter(
    new[] { Android.Content.Intent.ActionView },
    Categories = new[] { Android.Content.Intent.CategoryDefault, Android.Content.Intent.CategoryBrowsable },
    DataScheme = RedirectScheme)]
public class WebAuthenticationBrokerActivity : Uno.AuthenticationBroker.WebAuthenticationBrokerActivityBase
{
    /// <summary>The scheme of the RedirectUri in the OidcAuthentication configuration section.</summary>
    private const string RedirectScheme = "oidcdemo";
}
