using UXDivers.Grial;

namespace GrialKitApp;

public static class AppBuilderExtensions
{
    public static MauiAppBuilder UseMauiControls(this MauiAppBuilder builder)
    {
#if __IOS__
        var theme = new ThemeColorsBase(new Dictionary<string, Color>
        {
                { "AccentColor", Color.FromArgb("#FF3F75FF") }
        });

        GrialKit.Init(theme, "GrialKitApp.MauiControls.GrialLicense");
#else
        GrialKit.Init("GrialKitApp.MauiControls.GrialLicense");
#endif
        return builder
            .UseGrial()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Assets/Fonts/OpenSansRegular.ttf", "OpenSansRegular");
                fonts.AddFont("Assets/Fonts/OpenSansSemibold.ttf", "OpenSansSemibold");
            });
    }
}
