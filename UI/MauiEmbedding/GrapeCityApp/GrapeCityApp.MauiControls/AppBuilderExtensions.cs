using C1.Maui.Calendar;
using C1.Maui.Grid;

namespace GrapeCityApp;

public static class AppBuilderExtensions
{
	public static MauiAppBuilder UseMauiControls(this MauiAppBuilder builder) =>
		builder
	        .RegisterCalendarControls()
            .RegisterFlexGridControls()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("GrapeCityApp/Assets/Fonts/OpenSansRegular.ttf", "OpenSansRegular");
				fonts.AddFont("GrapeCityApp/Assets/Fonts/OpenSansSemibold.ttf", "OpenSansSemibold");
			});
}