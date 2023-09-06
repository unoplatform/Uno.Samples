namespace GrialKitApp;

public static class AppBuilderExtensions
{
	public static MauiAppBuilder UseMauiControls(this MauiAppBuilder builder) =>
		builder
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("GrialKitApp/Assets/Fonts/OpenSansRegular.ttf", "OpenSansRegular");
				fonts.AddFont("GrialKitApp/Assets/Fonts/OpenSansSemibold.ttf", "OpenSansSemibold");
			});
}