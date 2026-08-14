using Foundation;
using Microsoft.Identity.Client;
using UIKit;

namespace Authentication.MsalExtensionsDemo.iOS;

public class MsalAppDelegate : Uno.UI.Runtime.Skia.AppleUIKit.UnoUIApplicationDelegate
{
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
