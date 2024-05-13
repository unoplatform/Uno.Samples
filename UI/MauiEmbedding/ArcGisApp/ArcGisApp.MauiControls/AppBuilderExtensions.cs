using Esri.ArcGISRuntime;
using Esri.ArcGISRuntime.Maui;
using Esri.ArcGISRuntime.Security;

namespace ArcGisApp;

public static class AppBuilderExtensions
{
    public static MauiAppBuilder UseMauiControls(this MauiAppBuilder builder) =>
        builder
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("ArcGisApp/Assets/Fonts/OpenSansRegular.ttf", "OpenSansRegular");
                fonts.AddFont("ArcGisApp/Assets/Fonts/OpenSansSemibold.ttf", "OpenSansSemibold");
            })
            .UseArcGISRuntime(
            //config => config
            //    .UseLicense("[Your ArcGIS Maps SDK License key]")
            //    .UseApiKey("[Your ArcGIS location services API Key]")
            //    .ConfigureAuthentication(auth => auth
            //        .UseDefaultChallengeHandler() // Use the default authentication dialog
            //    )
            );
}
