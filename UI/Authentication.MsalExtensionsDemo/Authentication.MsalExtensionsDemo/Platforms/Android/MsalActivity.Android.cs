using Android.App;
using Android.Content;
using Android.Content.PM;
using Microsoft.Identity.Client;

namespace Authentication.MsalExtensionsDemo.Droid;

[Activity(
    Exported = true,
    LaunchMode = LaunchMode.SingleTask,
    NoHistory = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
[IntentFilter(
    [Intent.ActionView],
    Categories = [Intent.CategoryBrowsable, Intent.CategoryDefault],
    DataScheme = Authentication.MsalExtensionsDemo.Authentication.MsalConfig.AndroidRedirectScheme,
    DataHost = "auth")]
public class MsalActivity : BrowserTabActivity
{
}
