using CommunityToolkit.Maui;

namespace MauiEmbeddingApp;

public static class AppBuilderExtensions
{
    public static MauiAppBuilder UseMauiControls(this MauiAppBuilder builder)
        => builder
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkit(options => options.SetShouldEnableSnackbarOnWindows(true))
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("MauiCommunityToolkitApp/Assets/Fonts/OpenSansRegular.ttf", "OpenSansRegular");
                fonts.AddFont("MauiCommunityToolkitApp/Assets/Fonts/OpenSansSemibold.ttf", "OpenSansSemibold");
            });
}
