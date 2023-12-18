using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using Uno.Extensions.Markup;
using Uno.Toolkit.UI;

namespace TubePlayer.Presentation;

public partial class VideoDetailsPage : Page
{
    private MediaPlayerElement? youtubePlayer;

    public VideoDetailsPage()
    {
        this.DataContext<BindableVideoDetailsModel>((page, vm) => page
            .Background(Theme.Brushes.Background.Default)
            .NavigationCacheMode(NavigationCacheMode.Required)
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
                    .Add("Icon_Bar_Chart", "F1 M 0 3.1499998569488525 L 2.25 3.1499998569488525 L 2.25 10.5 L 0 10.5 L 0 3.1499998569488525 Z M 4.200000286102295 0 L 6.299999713897705 0 L 6.299999713897705 10.5 L 4.200000286102295 10.5 L 4.200000286102295 0 Z M 8.40000057220459 6 L 10.5 6 L 10.5 10.5 L 8.40000057220459 10.5 L 8.40000057220459 6 Z")
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
                                    .Content("Video")
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
                                new MediaPlayerElement()
                                    .Assign(mediaPlayerElement => youtubePlayer = mediaPlayerElement)
                                    .AutoPlay(true)
                                    .Source(() => vm.VideoSource)
                                    .PosterSource(() => vm.Video.Details.Snippet?.Thumbnails?.Medium?.Url!)
                                    .AreTransportControlsEnabled(true)
                                    .Width(400)
                                    .Height(300)
                                    .AutoLayout(counterAlignment: AutoLayoutAlignment.Start)
                                    .TransportControls
                                    (
                                        new MediaTransportControls()

                                    ),
                                new ScrollViewer()
                                    .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
                                    .Content
                                    (
                                        new AutoLayout()
                                            .Children
                                            (
                                                new AutoLayout()
                                                    .Spacing(6)
                                                    .Padding(16)
                                                    .Width(400)
                                                    .AutoLayout(counterAlignment: AutoLayoutAlignment.Start)
                                                    .Children
                                                    (
                                                        new TextBlock()
                                                            .TextWrapping(TextWrapping.Wrap)
                                                            .Text(() => vm.Video.Channel.Snippet?.Title)
                                                            .Foreground(Theme.Brushes.OnSurface.Default)
                                                            .Style(Theme.TextBlock.Styles.TitleLarge),
                                                        new TextBlock()
                                                            .Text(() => vm.Video.FormattedStatistics)
                                                            .Foreground(Theme.Brushes.OnSurface.Medium)
                                                            .AutoLayout(counterAlignment: AutoLayoutAlignment.Start)
                                                    ),
                                                new AutoLayout()
                                                    .Spacing(8)
                                                    .Orientation(Orientation.Horizontal)
                                                    .Padding(16, 8)
                                                    .Width(400)
                                                    .AutoLayout(counterAlignment: AutoLayoutAlignment.Start)
                                                    .Children
                                                    (
                                                        new Border()
                                                            .Width(40)
                                                            .Height(40)
                                                            .CornerRadius(20)
                                                            .AutoLayout(counterAlignment: AutoLayoutAlignment.Center)
                                                            .Child
                                                            (
                                                                new Image()
                                                                    .Source(() => vm.Video.Channel.Snippet?.Thumbnails?.High?.Url!)
                                                                    .Stretch(Stretch.UniformToFill)
                                                            ),
                                                        new AutoLayout()
                                                            .Justify(AutoLayoutJustify.SpaceBetween)
                                                            .Orientation(Orientation.Horizontal)
                                                            .Height(37)
                                                            .AutoLayout
                                                            (
                                                                counterAlignment: AutoLayoutAlignment.Center,
                                                                primaryAlignment: AutoLayoutPrimaryAlignment.Stretch
                                                            )
                                                            .Children
                                                            (
                                                                new AutoLayout()
                                                                    .PrimaryAxisAlignment(AutoLayoutAlignment.Center)
                                                                    .Children
                                                                    (
                                                                        new AutoLayout()
                                                                            .Orientation(Orientation.Horizontal)
                                                                            .AutoLayout
                                                                            (
                                                                                counterAlignment: AutoLayoutAlignment.Start,
                                                                                primaryAlignment: AutoLayoutPrimaryAlignment.Stretch
                                                                            )
                                                                            .Children
                                                                            (
                                                                                new TextBlock()
                                                                                    .Text(b => b.Bind(() => vm.Video.FormattedSubscriberCount))
                                                                                    .Foreground(Theme.Brushes.OnSurface.Medium)
                                                                                    .AutoLayout(counterAlignment: AutoLayoutAlignment.Start)
                                                                            ),
                                                                        new TextBlock()
                                                                            .Text(b => b.Bind(() => vm.Video.Channel.Snippet?.Title))
                                                                            .Foreground(Theme.Brushes.OnSurface.Default)
                                                                            .Style(Theme.TextBlock.Styles.TitleMedium)
                                                                            .AutoLayout(counterAlignment: AutoLayoutAlignment.Start)
                                                                    )
                                                                    , new Button()
                                                                    .Navigation(request: "VideoAnalytics")
                                                                    .Content("Stats")
                                                                    .AutoLayout(counterAlignment: AutoLayoutAlignment.Center)
                                                                    .ControlExtensions
                                                                    (
                                                                        icon:
                                                                            new PathIcon()
                                                                                .Data(StaticResource.Get<Geometry>("Icon_Bar_Chart"))
                                                                    )
                                                            )
                                                    ),
                                                new TextBlock()
                                                    .TextWrapping(TextWrapping.Wrap)
                                                    .Text(() => vm.Video.Channel.Snippet?.Description)
                                                    .Margin(16)
                                                    .Width(400)
                                                    .Foreground(Theme.Brushes.OnSurface.Medium)
                                                    .Style(Theme.TextBlock.Styles.BodySmall)
                                                    .AutoLayout(counterAlignment: AutoLayoutAlignment.Start)
                                            )
                                    )
                            )
                    )
            ))
            ;
    }

    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
        base.OnNavigatingFrom(e);

        youtubePlayer?.MediaPlayer.Pause();
    }
}
