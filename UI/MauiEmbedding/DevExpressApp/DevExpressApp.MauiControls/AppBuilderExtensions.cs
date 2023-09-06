// TODO: Uncomment
//using DevExpress.Maui;

namespace DevExpressApp;

public static class AppBuilderExtensions
{
	public static MauiAppBuilder UseMauiControls(this MauiAppBuilder builder) =>
		builder
            // TODO: Uncomment
            //.UseDevExpress()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("DevExpressApp/Assets/Fonts/OpenSansRegular.ttf", "OpenSansRegular");
                fonts.AddFont("DevExpressApp/Assets/Fonts/OpenSansSemibold.ttf", "OpenSansSemibold");
            });
}