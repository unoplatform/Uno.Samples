namespace UnoPongWars.Presentation;

public sealed partial class Shell : UserControl, IContentControlProvider
{
    public Shell()
    {
        this.Content(
            new Border()
                .Child(
                    new ExtendedSplashScreen()
                        .Name(out var splash)
                        .HorizontalAlignment(HorizontalAlignment.Stretch)
                        .VerticalAlignment(VerticalAlignment.Stretch)
                        .HorizontalContentAlignment(HorizontalAlignment.Stretch)
                        .VerticalContentAlignment(VerticalAlignment.Stretch)
                        .LoadingContentTemplate<object>(_ => new Grid()
                            .RowDefinitions("2*,*")
                            .Children(
                                new ProgressRing()
                                    .Grid(row: 1)
                                    .VerticalAlignment(VerticalAlignment.Center)
                                    .HorizontalAlignment(HorizontalAlignment.Center)
                                    .Height(100)
                                    .Width(100)
                            )
                        )
                )
                .Background(Theme.Brushes.Background.Default)
            );
        ContentControl = splash;
    }

    public ContentControl ContentControl { get; }
}
