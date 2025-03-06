namespace TelerikApp;

public static class AppBuilderExtensions
{
	public static MauiAppBuilder UseMauiControls(this MauiAppBuilder builder) =>
		builder
#if HAS_TELERIK
			.UseTelerik()
#endif
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("Assets/Fonts/OpenSansRegular.ttf", "OpenSansRegular");
				fonts.AddFont("Assets/Fonts/OpenSansSemibold.ttf", "OpenSansSemibold");
			});
}
