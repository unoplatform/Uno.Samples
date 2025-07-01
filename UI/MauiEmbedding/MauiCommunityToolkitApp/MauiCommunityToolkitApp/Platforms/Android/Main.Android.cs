using Android.Runtime;

namespace MauiCommunityToolkitApp.Droid;

[global::Android.App.ApplicationAttribute(
	Label = "@string/ApplicationName",
	Icon = "@mipmap/icon",
	LargeHeap = true,
	HardwareAccelerated = true,
	Theme = "@style/Theme.App.Starting"
)]
public class Application : Microsoft.UI.Xaml.NativeApplication
{
	static Application()
	{
		App.InitializeLogging();
	}

	public Application(IntPtr javaReference, JniHandleOwnership transfer)
		: base(() => new App(), javaReference, transfer)
	{
	}

}

