namespace TelerikApp;

public static class AppBuilderExtensions
{
	public static MauiAppBuilder UseMauiControls(this MauiAppBuilder builder) =>
		builder
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("TelerikApp/Assets/Fonts/OpenSansRegular.ttf", "OpenSansRegular");
				fonts.AddFont("TelerikApp/Assets/Fonts/OpenSansSemibold.ttf", "OpenSansSemibold");
			});
}