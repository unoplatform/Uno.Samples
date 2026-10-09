using System;
using Foundation;
using UIKit;
using Uno.UI.Xaml;

namespace ToyCar.iOS;

/// <summary>
/// iOS 27 refuses to launch an app built against its SDK unless the app adopts the UIScene
/// lifecycle. Uno's iOS host still creates its window in FinishedLaunching without attaching
/// it to a scene, so the scene would otherwise have nothing to display: this takes the host's
/// window and hands it to the scene once both exist.
/// </summary>
/// <remarks>
/// Temporary, and specific to this sample. Uno gains a built-in UnoUISceneDelegate in 7.0
/// (unoplatform/uno#19083), and the app template adopts the scene lifecycle in
/// unoplatform/uno.templates#2227. Once that support reaches the version this sample
/// targets, this file and the Info.plist scene manifest can both be removed.
/// </remarks>
[Register(nameof(SceneDelegate))]
public class SceneDelegate : UIResponder, IUIWindowSceneDelegate
{
	private const double WindowPollMilliseconds = 50;

	[Export("window")]
	public UIWindow? Window { get; set; }

	[Export("scene:willConnectToSession:options:")]
	public void WillConnect(UIScene scene, UISceneSession session, UISceneConnectionOptions connectionOptions)
	{
		if (scene is not UIWindowScene windowScene)
		{
			return;
		}

		AttachWhenReady(windowScene);
	}

	// App.OnLaunched may not have created the window yet when the scene connects, so this
	// retries rather than giving up on the first miss.
	private void AttachWhenReady(UIWindowScene windowScene)
	{
		if (Microsoft.UI.Xaml.Window.Current?.GetNativeWindow() as UIWindow is { } window)
		{
			window.WindowScene = windowScene;
			Window = window;
			window.MakeKeyAndVisible();
			return;
		}

		NSTimer.CreateScheduledTimer(TimeSpan.FromMilliseconds(WindowPollMilliseconds), _ => AttachWhenReady(windowScene));
	}
}
