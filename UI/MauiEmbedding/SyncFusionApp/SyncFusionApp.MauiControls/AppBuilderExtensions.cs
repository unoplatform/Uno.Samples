using Syncfusion.Maui.Core.Hosting;

namespace SyncfusionApp;

public static class AppBuilderExtensions
{
	public static MauiAppBuilder UseMauiControls(this MauiAppBuilder builder) =>
		builder
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("SyncfusionApp/Assets/Fonts/OpenSansRegular.ttf", "OpenSansRegular");
				fonts.AddFont("SyncfusionApp/Assets/Fonts/OpenSansSemibold.ttf", "OpenSansSemibold");
			})
			.ConfigureSyncfusionCore();
}