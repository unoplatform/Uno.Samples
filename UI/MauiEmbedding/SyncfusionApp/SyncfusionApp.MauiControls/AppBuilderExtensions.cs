using Syncfusion.Maui.Core.Hosting;

namespace SyncfusionApp;

public static class AppBuilderExtensions
{
	public static MauiAppBuilder UseMauiControls(this MauiAppBuilder builder) =>
		builder
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("Assets/Fonts/OpenSansRegular.ttf", "OpenSansRegular");
				fonts.AddFont("Assets/Fonts/OpenSansSemibold.ttf", "OpenSansSemibold");
			})
			.ConfigureSyncfusionCore();
}