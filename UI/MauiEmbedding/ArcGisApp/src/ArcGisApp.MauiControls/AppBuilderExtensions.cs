using Esri.ArcGISRuntime;
using Esri.ArcGISRuntime.Maui;
using Esri.ArcGISRuntime.Security;

namespace ArcGisApp;

public static class AppBuilderExtensions
{
    public static MauiAppBuilder UseMauiControls(this MauiAppBuilder builder) =>
        builder
            .UseArcGISRuntime(
            //config => config
            //    .UseLicense("[Your ArcGIS Maps SDK License key]")
            //    .UseApiKey("[Your ArcGIS location services API Key]")
            //    .ConfigureAuthentication(auth => auth
            //        .UseDefaultChallengeHandler() // Use the default authentication dialog
            //    )
            )
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Assets/Fonts/OpenSansRegular.ttf", "OpenSansRegular");
                fonts.AddFont("Assets/Fonts/OpenSansSemibold.ttf", "OpenSansSemibold");
            });
}
