// TODO: Uncomment using statements
//using C1.Maui.Calendar;
//using C1.Maui.Grid;

namespace GrapeCityApp;

public static class AppBuilderExtensions
{
	public static MauiAppBuilder UseMauiControls(this MauiAppBuilder builder) =>
		builder
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("HandlerMauiBlankApp/Assets/Fonts/OpenSansRegular.ttf", "OpenSansRegular");
                fonts.AddFont("HandlerMauiBlankApp/Assets/Fonts/OpenSansSemibold.ttf", "OpenSansSemibold");
            })
            // TODO: Uncomment Register method for the C1 Controls
            //.RegisterFlexGridControls()
	        //.RegisterCalendarControls()
        ;
}