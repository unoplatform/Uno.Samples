using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using Uno.Extensions.Markup;
using Uno.Extensions.Navigation.UI;
using Uno.Material;
using Uno.Themes;
using Uno.Toolkit.UI;
using Path = Microsoft.UI.Xaml.Shapes.Path;

namespace TubePlayer.Presentation;

public partial class MainPage : Page
{
    public MainPage()
    {
        this.DataContext<MainViewModel>((page, vm) => page
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
                    .Add("Icon_Chevron_Right", "F1 M 1.4099998474121094 0 L 0 1.4099998474121094 L 4.579999923706055 6 L 0 10.59000015258789 L 1.4099998474121094 12 L 7.409999847412109 6 L 1.4099998474121094 0 Z")
                    .Add("Icon_Search", "F1 M 12.5 11 L 11.710000038146973 11 L 11.430000305175781 10.729999542236328 C 12.410000324249268 9.589999556541443 13 8.110000014305115 13 6.5 C 13 2.9100000858306885 10.089999914169312 0 6.5 0 C 2.9100000858306885 0 0 2.9100000858306885 0 6.5 C 0 10.089999914169312 2.9100000858306885 13 6.5 13 C 8.110000014305115 13 9.589999556541443 12.410000324249268 10.729999542236328 11.430000305175781 L 11 11.710000038146973 L 11 12.5 L 16 17.489999771118164 L 17.489999771118164 16 L 12.5 11 L 12.5 11 Z M 6.5 11 C 4.009999990463257 11 2 8.990000009536743 2 6.5 C 2 4.009999990463257 4.009999990463257 2 6.5 2 C 8.990000009536743 2 11 4.009999990463257 11 6.5 C 11 8.990000009536743 8.990000009536743 11 6.5 11 Z")
            )
            .Content
            (
                new AutoLayout()
                    .PrimaryAxisAlignment(AutoLayoutAlignment.Center)
                    .VerticalAlignment(VerticalAlignment.Stretch)
                    .HorizontalAlignment(HorizontalAlignment.Center)
                    .Width(400)
                    .Children
                    (
                        new NavigationBar()
                            .Width(400)
                            .AutoLayout(counterAlignment: AutoLayoutAlignment.Center)
                            .Content
                            (
                                new AutoLayout()
                                    .PrimaryAxisAlignment(AutoLayoutAlignment.Center)
                                    .Orientation(Orientation.Horizontal)
                                    .Children
                                    (
                                        new Image()
                                            .Source("ms-appx:///Assets/navigation_bar.png")
                                            .Stretch(Stretch.UniformToFill)
                                            .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
                                    )
                            ),
                        new AutoLayout()
                            .Background(Theme.Brushes.Surface.Default)
                            .Padding(12)
                            .Children
                            (
                                new TextBox()
                                    .Background(Theme.Brushes.Surface.Variant.Default)
                                    .Text(b => b.Binding(() => vm.SearchTerm).TwoWay().UpdateSourceTrigger(UpdateSourceTrigger.PropertyChanged))
                                    .PlaceholderText("Search")
                                    .CornerRadius(20)
                                    .BorderThickness(0)
                                    .Style(Theme.TextBox.Styles.Outlined)
                                    .ControlExtensions
                                    (
                                        icon:
                                            new PathIcon()
                                                .Data(StaticResource.Get<Geometry>("Icon_Search"))
                                                .Foreground(Theme.Brushes.OnSurface.Variant.Default)
                                    )
                            ),
                            new FeedView()
                                .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
                                .VerticalAlignment(VerticalAlignment.Stretch)
                                .VerticalContentAlignment(VerticalAlignment.Stretch)
                                .Source(() => vm.VideoSearchResults)
                                .NoneTemplate(VideoNoneTemplate)
                                .ErrorTemplate(VideoErrorTemplate)
                                .ValueTemplate<FeedViewState>(feedViewState =>
                                    new ListView()
                                        .IsItemClickEnabled(true)
                                        .Background(Theme.Brushes.Background.Default)
                                        .ItemsSource(() => feedViewState.Data)
                                        .Padding(12, 8)
                                        .Navigation(request: "VideoDetails")
                                        .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
                                        .ItemTemplate<YoutubeVideo>(VideoItemTemplate)
                                )
                    )
            ))
            ;
    }

    private static UIElement VideoItemTemplate(YoutubeVideo youtubeVideo) =>
        new CardContentControl()
            .Margin(0, 0, 0, 8)
            .Style(StaticResource.Get<Style>("ElevatedCardContentControlStyle"))
            .Content
            (
                new AutoLayout()
                    .Background(Theme.Brushes.Surface.Default)
                    .CornerRadius(12)
                    .PrimaryAxisAlignment(AutoLayoutAlignment.Center)
                    .Children
                    (
                        new AutoLayout()
                            .Background(Theme.Brushes.Surface.Default)
                            .CornerRadius(12)
                            .Padding(8, 8, 8, 0)
                            .MaxHeight(288)
                            .MaxWidth(456)
                            .AutoLayout(counterAlignment: AutoLayoutAlignment.Center)
                            .Children
                            (
                                new Border()
                                    .Height(204.75)
                                    .CornerRadius(6)
                                    .Child
                                    (
                                        new Image()
                                            .Source(() => youtubeVideo.Details.Snippet?.Thumbnails?.Medium?.Url!)
                                            .Stretch(Stretch.UniformToFill)
                                    ),
                                new AutoLayout()
                                    .Spacing(8)
                                    .Orientation(Orientation.Horizontal)
                                    .Padding(0, 8)
                                    .Children
                                    (
                                        new AutoLayout()
                                            .CornerRadius(30)
                                            .Orientation(Orientation.Horizontal)
                                            .Height(60)
                                            .AutoLayout(counterAlignment: AutoLayoutAlignment.Center)
                                            .Children
                                            (
                                                new Border()
                                                    .Width(60)
                                                    .Height(60)
                                                    .CornerRadius(30)
                                                    .AutoLayout(counterAlignment: AutoLayoutAlignment.Start)
                                                    .Child
                                                    (
                                                        new Image()
                                                            .Source(() => youtubeVideo.Channel.Snippet?.Thumbnails?.Medium?.Url!)
                                                            .Stretch(Stretch.UniformToFill)
                                                    )
                                            ),
                                        new AutoLayout()
                                            .PrimaryAxisAlignment(AutoLayoutAlignment.Center)
                                            .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
                                            .Children
                                            (
                                                new TextBlock()
                                                    .Text(() => youtubeVideo.Channel.Snippet?.Title)
                                                    .Height(22)
                                                    .Foreground(Theme.Brushes.OnSurface.Default)
                                                    .Style(Theme.TextBlock.Styles.TitleMedium),
                                                new TextBlock()
                                                    .Text(() => youtubeVideo.Details.Snippet?.Title)
                                                    .Height(16)
                                                    .Foreground(Theme.Brushes.OnSurface.Medium)
                                            ),
                                        new Button()
                                            .Foreground(Theme.Brushes.OnSurface.Medium)
                                            .Style(Theme.Button.Styles.Icon)
                                            .AutoLayout(counterAlignment: AutoLayoutAlignment.Center)
                                            .Content
                                            (
                                                new PathIcon()
                                                    .Data(StaticResource.Get<Geometry>("Icon_Chevron_Right"))
                                                    .Foreground(Theme.Brushes.OnSurface.Medium)
                                            )
                                    )
                            )
                    )
            );

    private static UIElement VideoNoneTemplate() =>
        new AutoLayout()
            .Spacing(24)
            .PrimaryAxisAlignment(AutoLayoutAlignment.Center)
            .Padding(0, 0, 0, 122)
            .AutoLayout
            (
                counterAlignment: AutoLayoutAlignment.Center,
                primaryAlignment: AutoLayoutPrimaryAlignment.Stretch
            )
            .Children
            (
                new AutoLayout()
                    .Spacing(10)
                    .PrimaryAxisAlignment(AutoLayoutAlignment.Center)
                    .Width(160)
                    .Height(160)
                    .AutoLayout(counterAlignment: AutoLayoutAlignment.Center)
                    .Children
                    (
                        new Ellipse()
                            .Fill(Theme.Brushes.Surface.Default)
                            .Width(160)
                            .Height(160)
                            .AutoLayout(counterAlignment: AutoLayoutAlignment.Center),
                        new Path()
                            .Data("F1 M 36.57652282714844 15.768688201904297 L 28.12342071533203 24.199007034301758 L 19.670324325561523 15.768688201904297 L 15.811301231384277 19.6173095703125 L 24.264400482177734 28.047626495361328 L 15.811301231384277 36.47794723510742 L 19.670324325561523 40.326568603515625 L 28.12342071533203 31.896251678466797 L 36.57652282714844 40.326568603515625 L 40.435546875 36.47794723510742 L 31.982446670532227 28.047626495361328 L 40.435546875 19.6173095703125 L 36.57652282714844 15.768688201904297 Z M 49.807456970214844 45.64133071899414 C 54.034006118774414 40.693100929260254 56.239158630371094 34.46199989318848 56.239158630371094 27.864360809326172 C 56.239158630371094 20.533650398254395 53.29895353317261 13.38620662689209 47.969825744628906 8.254709243774414 C 42.824461460113525 2.939943790435791 35.65770435333252 -0.17560786567628384 28.12342071533203 0.0076598916202783585 C 20.589137077331543 0.0076598916202783585 13.422379970550537 2.939943790435791 8.277015686035156 8.254709243774414 C 2.947887897491455 13.38620662689209 -0.17608242109417915 20.53364849090576 0.007680591195821762 28.047626495361328 C 0.007680591195821762 35.561604499816895 2.947887897491455 42.7090482711792 8.277015686035156 47.840545654296875 C 13.422379970550537 53.1553111076355 20.589137077331543 56.27085702121258 28.12342071533203 56.087589263916016 C 34.55512619018555 56.087589263916016 40.986831188201904 53.88838005065918 45.94843292236328 49.673221588134766 L 64.32473754882812 68 L 68 63.96811294555664 L 49.807456970214844 45.64133071899414 Z M 44.110801696777344 43.991920471191406 C 39.88425254821777 48.39034700393677 34.18760013580322 50.772829219698906 28.12342071533203 50.589561462402344 C 22.05924129486084 50.772829219698906 16.36258888244629 48.20707893371582 12.136039733886719 43.991920471191406 C 7.725727081298828 39.77676200866699 5.33680821955204 34.09546232223511 5.520571231842041 28.047626495361328 C 5.33680821955204 21.99979066848755 7.909490585327148 16.318490982055664 12.136039733886719 12.10333251953125 C 16.36258888244629 7.704905986785889 22.05924129486084 5.322425201535225 28.12342071533203 5.505692958831787 C 34.18760013580322 5.505692958831787 39.88425254821777 7.888174057006836 44.110801696777344 12.10333251953125 C 48.521114349365234 16.318490982055664 50.91003559529781 21.99979066848755 50.72627258300781 28.047626495361328 C 50.72627258300781 34.09546232223511 48.337350845336914 39.77676200866699 44.110801696777344 43.991920471191406 Z")
                            .Fill(Theme.Brushes.Secondary.Default)
                            .Margin(46, 46, 0, 0)
                            .VerticalAlignment(VerticalAlignment.Top)
                            .HorizontalAlignment(HorizontalAlignment.Left)
                            .Width(68)
                            .Height(68)
                            .AutoLayout(isIndependentLayout: true)
                    ),
                new AutoLayout()
                    .PrimaryAxisAlignment(AutoLayoutAlignment.Center)
                    .AutoLayout(counterAlignment: AutoLayoutAlignment.Center)
                    .Children
                    (
                        new TextBlock()
                            .Text("No results found")
                            .Foreground(Theme.Brushes.OnSurface.Default)
                            .Style(Theme.TextBlock.Styles.HeadlineSmall)
                            .AutoLayout(counterAlignment: AutoLayoutAlignment.Center),
                        new TextBlock()
                            .Text("Try a new search")
                            .Foreground(Theme.Brushes.OnSurface.Medium)
                            .Style(Theme.TextBlock.Styles.BodyLarge)
                            .AutoLayout(counterAlignment: AutoLayoutAlignment.Center)
                    )
            );

    private static UIElement VideoErrorTemplate() =>
        new AutoLayout()
            .Spacing(8)
            .PrimaryAxisAlignment(AutoLayoutAlignment.Center)
            .AutoLayout(primaryAlignment: AutoLayoutPrimaryAlignment.Stretch)
            .Children
            (
                new AutoLayout()
                    .Spacing(24)
                    .PrimaryAxisAlignment(AutoLayoutAlignment.Center)
                    .Padding(0, 0, 0, 36)
                    .AutoLayout
                    (
                        counterAlignment: AutoLayoutAlignment.Center,
                        primaryAlignment: AutoLayoutPrimaryAlignment.Stretch
                    )
                    .Children
                    (
                        new AutoLayout()
                            .Spacing(10)
                            .PrimaryAxisAlignment(AutoLayoutAlignment.Center)
                            .Width(160)
                            .Height(160)
                            .AutoLayout(counterAlignment: AutoLayoutAlignment.Center)
                            .Children
                            (
                                new Ellipse()
                                    .Fill(Theme.Brushes.Surface.Default)
                                    .Width(160)
                                    .Height(160)
                                    .AutoLayout(counterAlignment: AutoLayoutAlignment.Center),
                                new Path()
                                    .Data("F1 M 78.19999694824219 1.8000001907348633 C 77.19999694824219 0.6000001430511475 75.60000002384186 0 74 0 L 6 0 C 4.399999976158142 0 2.8000001907348633 0.6000001430511475 1.8000001907348633 1.8000001907348633 C 0.6000001430511475 2.8000001907348633 0 4.399999976158142 0 6 L 0 58 C 0 59.60000002384186 0.6000001430511475 61.19999694824219 1.8000001907348633 62.19999694824219 C 2.8000001907348633 63.3999969959259 4.399999976158142 64 6 64 L 74 64 C 77.20000004768372 64 80 61.200000047683716 80 58 L 80 6 C 80 4.399999976158142 79.3999969959259 2.8000001907348633 78.19999694824219 1.8000001907348633 Z M 74 58 L 6 58 L 6 6 L 74 6 L 74 58 Z M 30.799999237060547 45.400001525878906 L 40 36.20000076293945 L 49.20000076293945 45.400001525878906 L 53.400001525878906 41.20000076293945 L 44.20000076293945 32 L 53.400001525878906 22.799999237060547 L 49.20000076293945 18.600000381469727 L 40 27.799999237060547 L 30.799999237060547 18.600000381469727 L 26.600000381469727 22.799999237060547 L 35.79999923706055 32 L 26.600000381469727 41.20000076293945 L 30.799999237060547 45.400001525878906 Z")
                                    .Fill(Theme.Brushes.Secondary.Default)
                                    .Margin(40, 48)
                                    .VerticalAlignment(VerticalAlignment.Center)
                                    .HorizontalAlignment(HorizontalAlignment.Center)
                                    .Width(80)
                                    .Height(64)
                                    .AutoLayout(isIndependentLayout: true)
                            ),
                        new AutoLayout()
                            .PrimaryAxisAlignment(AutoLayoutAlignment.Center)
                            .AutoLayout(counterAlignment: AutoLayoutAlignment.Center)
                            .Children
                            (
                                new TextBlock()
                                    .Text("Oh no!")
                                    .Foreground(Theme.Brushes.OnSurface.Default)
                                    .Style(Theme.TextBlock.Styles.HeadlineSmall)
                                    .AutoLayout(counterAlignment: AutoLayoutAlignment.Center),
                                new TextBlock()
                                    .Text("Something went wrong")
                                    .Foreground(Theme.Brushes.OnSurface.Medium)
                                    .Style(Theme.TextBlock.Styles.BodyLarge)
                                    .AutoLayout(counterAlignment: AutoLayoutAlignment.Center)
                            )
                    ),
                new Button()
                    .Content("Retry")
                    .Margin(16, 24)
                    .Style(Theme.Button.Styles.FilledTonal)
                    .ControlExtensions
                    (
                        icon:
                            new PathIcon()
                                .Data(StaticResource.Get<Geometry>("Icon_Refresh"))
                    )
            );
}
