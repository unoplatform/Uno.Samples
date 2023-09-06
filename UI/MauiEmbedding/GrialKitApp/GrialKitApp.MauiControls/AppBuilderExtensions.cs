using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
// TODO: Uncomment
//using UXDivers.Grial;
using static System.Net.Mime.MediaTypeNames;

#if ANDROID
using Android.App;
using Android.Content;
using Android.Content.PM;
#endif

namespace GrialKitApp;

public static class AppBuilderExtensions
{
	public static MauiAppBuilder UseMauiControls(this MauiAppBuilder builder)
	{
        // TODO: Uncomment
        //#if __IOS__
        //		var theme = new ThemeColorsBase(new Dictionary<string, Color>
        //		{
        //			{ "AccentColor", Color.FromArgb("#FF3F75FF") }
        //		});

        //		GrialKit.Init(theme, "GrialKitApp.MauiControls.GrialLicense");
        //#else
        //        Context context = Android.App.Application.Context;

        //		var lbl = ((PackageItemInfo)context.ApplicationInfo).LoadLabel(context.PackageManager);
        //		var lbl2 = ((PackageItemInfo)context.ApplicationInfo).NonLocalizedLabel;

        //		GrialKit.Init("GrialKitApp.MauiControls.GrialLicense");
        //#endif
        return builder
            // TODO: Uncomment
            // .UseGrial()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("GrialKitApp/Assets/Fonts/OpenSansRegular.ttf", "OpenSansRegular");
				fonts.AddFont("GrialKitApp/Assets/Fonts/OpenSansSemibold.ttf", "OpenSansSemibold");
			});
	}
}