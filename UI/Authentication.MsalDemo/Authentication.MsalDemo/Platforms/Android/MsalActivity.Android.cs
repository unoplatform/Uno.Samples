using Android.App;
using Android.Content;
using Android.Content.PM;
using Microsoft.Identity.Client;

namespace Authentication.MsalDemo.Droid;

/// <summary>
/// Catches the redirect back from the Chrome custom tab after sign-in.
/// </summary>
/// <remarks>
/// <para>
/// MSAL.NET ships <see cref="BrowserTabActivity"/>; all this app has to do is declare an activity
/// that derives from it with an intent filter matching the redirect URI. Without a matching intent
/// filter, Android has nowhere to deliver the callback and <c>AcquireTokenInteractive</c> simply
/// never completes.
/// </para>
/// <para>
/// The scheme is <c>msal{ClientId}</c> and the host is <c>auth</c>, which together form the
/// <c>msal{ClientId}://auth</c> redirect URI that has to be registered in Entra ID. Because
/// <see cref="Authentication.MsalConfig.AndroidRedirectScheme"/> is a <c>const</c>, it can be used
/// in this attribute - so changing the client ID in one place updates the generated
/// AndroidManifest.xml too.
/// </para>
/// </remarks>
[Activity(
    Exported = true,
    LaunchMode = LaunchMode.SingleTask,
    NoHistory = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
[IntentFilter(
    [Intent.ActionView],
    Categories = [Intent.CategoryBrowsable, Intent.CategoryDefault],
    DataScheme = Authentication.MsalConfig.AndroidRedirectScheme,
    DataHost = "auth")]
public class MsalActivity : BrowserTabActivity
{
}
