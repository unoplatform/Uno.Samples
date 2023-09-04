using DevExpress.Maui;

namespace DevExpressApp;

public static class AppBuilderExtensions
{
	public static MauiAppBuilder UseMauiControls(this MauiAppBuilder builder) =>
		builder
            .UseDevExpress()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("DevExpressApp/Assets/Fonts/OpenSansRegular.ttf", "OpenSansRegular");
                fonts.AddFont("DevExpressApp/Assets/Fonts/OpenSansSemibold.ttf", "OpenSansSemibold");
            });
}