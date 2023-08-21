using C1.Maui.Calendar;
using C1.Maui.Grid;
using CommunityToolkit.Maui;

namespace GrapeCityApp;

public static class AppBuilderExtensions
{
	public static MauiAppBuilder UseMauiControls(this MauiAppBuilder builder) =>
		builder.UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("HandlerMauiBlankApp/Assets/Fonts/OpenSansRegular.ttf", "OpenSansRegular");
                fonts.AddFont("HandlerMauiBlankApp/Assets/Fonts/OpenSansSemibold.ttf", "OpenSansSemibold");
            })
            .RegisterFlexGridControls()
	        .RegisterCalendarControls();
}