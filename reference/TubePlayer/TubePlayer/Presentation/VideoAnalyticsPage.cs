using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using TubePlayer.MauiControls;
using Uno.Extensions.Markup;
using Uno.Toolkit.UI;

namespace TubePlayer.Presentation;

public partial class VideoAnalyticsPage : Page
{
    public VideoAnalyticsPage()
    {
        this
            .Background(Theme.Brushes.Background.Default)
            .StatusBar
            (
                s => s
                    .Foreground(StatusBarForegroundTheme.Auto)
                    .Background(Theme.Brushes.Surface.Default)
            )
            .Resources
            (
                r => r
                    .Add("Icon_Arrow_Back", "F1 M 16 7 L 3.8299999237060547 7 L 9.420000076293945 1.4099998474121094 L 8 0 L 0 8 L 8 16 L 9.40999984741211 14.59000015258789 L 3.8299999237060547 9 L 16 9 L 16 7 Z")
            )
            .Content
            (
                new AutoLayout()
                    .Background(Theme.Brushes.Background.Default)
                    .Children
                    (
                        new AutoLayout()
                            .Width(400)
                            .AutoLayout
                            (
                                counterAlignment: AutoLayoutAlignment.Center,
                                primaryAlignment: AutoLayoutPrimaryAlignment.Stretch
                            )
                            .Children
                            (
                                new NavigationBar()
                                    .HorizontalContentAlignment(HorizontalAlignment.Left)
                                    .Content("Video Stats")
                                    .MainCommand
                                    (
                                        new AppBarButton()
                                            .Icon
                                            (
                                                new PathIcon()
                                                    .Data(StaticResource.Get<Geometry>("Icon_Arrow_Back"))
                                                    .Foreground(Theme.Brushes.OnSurface.Default)
                                            )
                                    ),
                                new Grid()
                                    .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
                                    .Children
                                    (
                                            // EMBEDDED MAUI CONTENT
                                            new MauiHost().Source(typeof(EmbeddedControl))
                                    )
                            )
                    )
            )
            ;
    }
}

public static class MauiHostExtensions
{
    public static MauiHost Source(this MauiHost host, Type sourceType)
    {
        host.Source = sourceType;
        return host;
    }
}