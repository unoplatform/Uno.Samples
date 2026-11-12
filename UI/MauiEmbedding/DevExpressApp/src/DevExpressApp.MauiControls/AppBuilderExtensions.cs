using DevExpress.Maui;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics.Metrics;
using System.Xml.Linq;

namespace DevExpressApp;

public static class AppBuilderExtensions
{
    public static MauiAppBuilder UseMauiControls(this MauiAppBuilder builder) =>
    builder
            .UseDevExpress()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Assets/Fonts/OpenSansRegular.ttf", "OpenSansRegular");
                fonts.AddFont("Assets/Fonts/OpenSansSemibold.ttf", "OpenSansSemibold");
            });
}
