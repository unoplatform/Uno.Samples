namespace TubePlayer.Presentation;

public sealed partial class Shell : UserControl, IContentControlProvider
{
    public Shell()
    {
        this.Content(
            new Border()
                .Child(
                    new ExtendedSplashScreen()
                        .Assign(out var splash)
                        .HorizontalAlignment(HorizontalAlignment.Stretch)
                        .VerticalAlignment(VerticalAlignment.Stretch)
                        .HorizontalContentAlignment(HorizontalAlignment.Stretch)
                        .VerticalContentAlignment(VerticalAlignment.Stretch)
                        .LoadingContentTemplate<object>(_ => new Grid()
                            .RowDefinitions(new GridLength(2, GridUnitType.Star), new GridLength(1, GridUnitType.Star))
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
