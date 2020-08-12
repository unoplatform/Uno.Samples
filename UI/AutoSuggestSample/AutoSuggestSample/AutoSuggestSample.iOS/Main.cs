using UIKit;

namespace AutoSuggestSample.iOS
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main(string[] args)
		{
			Uno.UI.FeatureConfiguration.Control.UseLegacyLazyApplyTemplate = true;

			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main(args, null, typeof(App));
		}
	}
}