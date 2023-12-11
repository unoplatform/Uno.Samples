using Syncfusion.Maui.Core.Hosting;

namespace TubePlayer;

public static class AppBuilderExtensions
{
    public static MauiAppBuilder UseMauiControls(this MauiAppBuilder builder) =>
        builder
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("TubePlayer/Assets/Fonts/OpenSansRegular.ttf", "OpenSansRegular");
                fonts.AddFont("TubePlayer/Assets/Fonts/OpenSansSemibold.ttf", "OpenSansSemibold");
            })
            .ConfigureSyncfusionCore()
        ;
}
