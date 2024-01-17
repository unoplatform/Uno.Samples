using ChatGPT.Business;

namespace ChatGPT.Presentation;

public sealed partial class MainPage : Page
{
    public MainPage(IServiceProvider services)
    {
        this.Background(ThemeResource.Get<Brush>("ApplicationPageBackgroundThemeBrush"))
            .DataContext(services.GetRequiredService<BindableMainModel>(), (page, vm) => page
                .Content(
                    new Grid().BorderBrush(Colors.Gray).BorderThickness(1).CornerRadius(10)
                        .MaxWidth(500)
                        .Padding(10).Margin(0, 10)
                        .RowDefinitions<Grid>("*,Auto")
                        .Children(
                            new ListView()
                                .VerticalAlignment(VerticalAlignment.Bottom)
                                .SelectionMode(ListViewSelectionMode.None)
                                .ItemsSource(() => vm.Messages)
                                .AutoScrollListViewBehavior(autoScroll: true)
                                .ItemTemplateSelector<Message>((message, selector) => selector
                                    .Default(() => new TextBlock().Text(() => message.Content))
                                    .Case(
                                        m => m.Source == Source.User,
                                        () => new StackPanel()
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
                                    .Case(
                                        m => m.Source == Source.AI && m.Status != Status.Error,
                                        () => new StackPanel()
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
                                                            .Text(() => message.Content)
                                                    )
                                            )
                                    )
                                    .Case(
                                        m => m.Source == Source.AI && m.Status == Status.Error,
                                        () => new StackPanel()
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
                                                            .Foreground(Colors.Red)
                                                            .HorizontalAlignment(HorizontalAlignment.Center)
                                                            .TextWrapping(TextWrapping.Wrap)
                                                            .Text("Error"))
                                            )
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
                                        .PlaceholderText("Message ChatGPT")
                                        .CommandExtensions(x => x.Command(() => vm.SendMessage))
                                        .Text(x => x.Bind(() => vm.Prompt).TwoWay().UpdateSourceTrigger(UpdateSourceTrigger.PropertyChanged)),
                                    new Button()
                                        .Content("Send")
                                        .Command(() => vm.SendMessage)
                            )
                    )
                )
            );
    }
}
