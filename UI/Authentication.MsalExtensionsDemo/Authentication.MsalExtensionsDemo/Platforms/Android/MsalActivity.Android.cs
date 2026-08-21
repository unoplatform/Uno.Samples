using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Microsoft.Identity.Client;

namespace Authentication.MsalExtensionsDemo.Droid;

/// <summary>
/// Catches the redirect back from the Chrome custom tab after sign-in.
/// </summary>
/// <remarks>
/// <para>
/// MSAL.NET ships <see cref="BrowserTabActivity"/>; all this app has to do is declare an activity
/// that derives from it with an intent filter matching the redirect URI. Without a matching intent
/// filter, Android has nowhere to deliver the callback and the interactive sign-in simply never
/// completes.
/// </para>
/// <para>
/// The redirect URI the MSAL provider derives on Android is <c>msal{ClientId}://auth</c>, so the
/// scheme below must be <c>msal</c> followed by the client ID configured in the
/// <c>MsalAuthentication</c> section of appsettings.
/// </para>
/// <para>
/// <b>This is the one value that cannot follow appsettings automatically:</b> intent filters are
/// declared with attributes, which only accept compile-time constants, so
/// <see cref="RedirectScheme"/> has to be updated by hand when the client ID changes.
/// </para>
/// </remarks>
[Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
[IntentFilter(
    [Intent.ActionView],
    Categories = [Intent.CategoryDefault, Intent.CategoryBrowsable],
    DataScheme = RedirectScheme,
    DataHost = "auth")]
public class MsalActivity : BrowserTabActivity
{
    /// <summary>
    /// Must be <c>"msal"</c> + the ClientId from the MsalAuthentication configuration section.
    /// The placeholder client ID below is deliberate: replace it with your own, exactly as it
    /// appears in appsettings, or Android sign-in will never return to the app.
    /// </summary>
    private const string RedirectScheme = "msal00000000-0000-0000-0000-000000000000";
}
