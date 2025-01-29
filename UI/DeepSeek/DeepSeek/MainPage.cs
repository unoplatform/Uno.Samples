namespace DeepSeek;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this
            .Background(Theme.Brushes.Background.Default)
            .Content(new StackPanel()
            .VerticalAlignment(VerticalAlignment.Center)
            .HorizontalAlignment(HorizontalAlignment.Center)
            .Children(
                new TextBlock()
                    .Text("Hello Uno Platform!")
            ));
    }
}
