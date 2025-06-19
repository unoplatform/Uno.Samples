using Android.Runtime;

namespace TubePlayer.Droid;

[global::Android.App.ApplicationAttribute(
    Label = "@string/ApplicationName",
    Icon = "@mipmap/icon",
    LargeHeap = true,
    HardwareAccelerated = true,
    Theme = "@style/AppTheme"
)]
public class Application(IntPtr javaReference, JniHandleOwnership transfer)
    : NativeApplication(() => new App(), javaReference, transfer);

