using CommunityToolkit.Maui;

namespace MauiEmbeddingApp;

public static class AppBuilderExtensions
{
    public static MauiAppBuilder UseMauiControls(this MauiAppBuilder builder)
        => builder
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("MauiCommunityToolkitApp/Assets/Fonts/OpenSansRegular.ttf", "OpenSansRegular");
                fonts.AddFont("MauiCommunityToolkitApp/Assets/Fonts/OpenSansSemibold.ttf", "OpenSansSemibold");
            });
}
