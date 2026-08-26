using Foundation;
using Microsoft.Identity.Client;
using UIKit;

namespace Authentication.MsalExtensionsDemo.iOS;

/// <summary>
/// Hands the sign-in callback URL back to MSAL.
/// </summary>
/// <remarks>
/// <para>
/// iOS delivers the <c>msauth.{BundleId}://auth</c> redirect to the app as an
/// <see cref="OpenUrl(UIApplication, NSUrl, NSDictionary)"/> call, and MSAL has a pending request
/// waiting for it. <c>AuthenticationContinuationHelper</c> is what connects the two. The scheme
/// itself must be declared under <c>CFBundleURLTypes</c> in <c>Info.plist</c>.
/// </para>
/// <para>
/// With Skia rendering the <c>App</c> class is not the <c>UIApplicationDelegate</c> any more, so
/// custom lifecycle handling goes in a type deriving from Uno's <c>UnoUIApplicationDelegate</c>,
/// registered in <c>Main.iOS.cs</c> with
/// <c>.UseAppleUIKit(b =&gt; b.UseUIApplicationDelegate&lt;MsalAppDelegate&gt;())</c>.
/// Always call the base implementation so Uno's own handling still runs.
/// </para>
/// </remarks>
public class MsalAppDelegate : Uno.UI.Runtime.Skia.AppleUIKit.UnoUIApplicationDelegate
{
    // CA1422: iOS 26 marks application:openURL:options: obsolete in favour of the UIScene
    // lifecycle (UISceneDelegate.OpenUrlContexts). This app does not declare a
    // UIApplicationSceneManifest in Info.plist, so it runs the application-delegate lifecycle and
    // this is the callback iOS actually invokes. If you adopt UIScene, move the same forwarding
    // call into your scene delegate's OpenUrlContexts.
#pragma warning disable CA1422
    public override bool OpenUrl(UIApplication application, NSUrl url, NSDictionary options)
    {
        if (AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(url))
        {
            return true;
        }

        return base.OpenUrl(application, url, options);
    }
#pragma warning restore CA1422
}
