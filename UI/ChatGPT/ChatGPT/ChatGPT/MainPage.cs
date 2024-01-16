using ChatGPT.Business;
using ChatGPT.Services;

namespace ChatGPT;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.Background(ThemeResource.Get<Brush>("ApplicationPageBackgroundThemeBrush"))
            .DataContext(new BindableMainModel(new ChatService()), (page, vm) => page
                .Content(new StackPanel()
                            .VerticalAlignment(VerticalAlignment.Bottom)
                            .HorizontalAlignment(HorizontalAlignment.Center)
                            .MaxWidth(700)
                            .Padding(0,10)
                            .Children(
                                new Grid()
                                    .RowDefinitions<Grid>("*,Auto")
                                    .Children(
                                        new ScrollViewer()
                                            .Grid(row: 0)
                                            .Content(
                                                new FeedView()
                                                    .Source(() => vm.Messages)
                                                    .ValueTemplate<FeedViewState>(feedViewState =>
                                                    new ListView()
                                                            .ItemTemplateSelector<Message>((message, selector) => selector
                                                                .Default(() => new TextBlock().Text(message.Content))
                                                                .Case(m => m.Source == Source.User, () => new StackPanel()
                                                                                                              .Margin(8)
                                                                                                              .HorizontalAlignment(HorizontalAlignment.Right)
                                                                                                              .Children(
                                                                                                                new Border()
                                                                                                                    .Background(Colors.DarkSlateGray)
                                                                                                                    .CornerRadius(10)
                                                                                                                    .MinWidth(70)
                                                                                                                    .MaxWidth(350)
                                                                                                                    .Padding(10)
                                                                                                                    .Child(
                                                                                                                        new TextBlock()
                                                                                                                          .Foreground(Colors.Yellow)
                                                                                                                          .HorizontalAlignment(HorizontalAlignment.Center)
                                                                                                                          .TextWrapping(TextWrapping.Wrap)
                                                                                                                          .Text(() => message.Content))
                                                                                                                    )
                                                                                                                )
                                                                .Case(m => m.Source == Source.AI, () => new StackPanel()
                                                                                                              .Margin(8)
                                                                                                              .HorizontalAlignment(HorizontalAlignment.Left)
                                                                                                              .Children(
                                                                                                                new Border()
                                                                                                                    .Background(Colors.DarkGray)
                                                                                                                    .CornerRadius(10)
                                                                                                                    .MinWidth(70)
                                                                                                                    .MaxWidth(350)
                                                                                                                    .Padding(10)
                                                                                                                    .Child(
                                                                                                                        new TextBlock()
                                                                                                                          .Foreground(Colors.Black)
                                                                                                                          .HorizontalAlignment(HorizontalAlignment.Center)
                                                                                                                          .TextWrapping(TextWrapping.Wrap)
                                                                                                                          .Text(() => message.Content))
                                                                                                                    )
                                                                                                                )
                                                            )
                                                            .ItemsSource(() => feedViewState.Data)
                                                    )
                                                    .ProgressTemplate<FeedViewState>(feedViewState =>
                                                        //TODO
                                                        new ProgressRing()
                                                    )
                                                    .ErrorTemplate<FeedViewState>(feedViewState =>
                                                        //TODO
                                                        new TextBlock().Text("An error has occurred retrieving messages")
                                                    )
                                                    .NoneTemplate<FeedViewState>(feedViewState =>
                                                        //TODO
                                                        new TextBlock().Text("Start a conversation")
                                                    )
                                            ),
                                        new StackPanel()
                                            .Grid(row: 1)
                                            .VerticalAlignment(VerticalAlignment.Bottom)
                                            .HorizontalAlignment(HorizontalAlignment.Center)
                                            .Orientation(Orientation.Horizontal)
                                            .Spacing(10)
                                            .Children(
                                                new TextBox()
                                                    .Width(300)
                                                    .Text(x => x.Bind(() => vm.UserMessage).TwoWay()),
                                                new Button()
                                                    .Content("Send")
                                                    .Command(() => vm.SendMessage)
                                        )
                                )
                            )
                )
            );
    }
}
