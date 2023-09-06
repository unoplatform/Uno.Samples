using Esri.ArcGISRuntime.Maui;

namespace ArcGisApp;

public static class AppBuilderExtensions
{
	public static MauiAppBuilder UseMauiControls(this MauiAppBuilder builder) =>
		builder
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("ArcGisApp/Assets/Fonts/OpenSansRegular.ttf", "OpenSansRegular");
				fonts.AddFont("ArcGisApp/Assets/Fonts/OpenSansSemibold.ttf", "OpenSansSemibold");
			})
	        .UseArcGISRuntime();
}