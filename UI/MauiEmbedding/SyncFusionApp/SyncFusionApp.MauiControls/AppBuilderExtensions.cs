using Syncfusion.Maui.Core.Hosting;

namespace SyncFusionApp;

public static class AppBuilderExtensions
{
	public static MauiAppBuilder UseMauiControls(this MauiAppBuilder builder) =>
		builder
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("SyncFusionApp/Assets/Fonts/OpenSansRegular.ttf", "OpenSansRegular");
				fonts.AddFont("SyncFusionApp/Assets/Fonts/OpenSansSemibold.ttf", "OpenSansSemibold");
			})
			.ConfigureSyncfusionCore();
}