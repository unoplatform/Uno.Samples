namespace Counter;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.DataContext(new BindableMainModel(), (page, vm) => page
            .Background(ThemeResource.Get<Brush>("ApplicationPageBackgroundThemeBrush"))
            .Content(
                new StackPanel()
                    .VerticalAlignment(VerticalAlignment.Center)
                    .Children(
                        new Image()
                            .Margin(12)
                            .HorizontalAlignment(HorizontalAlignment.Center)
                            .Width(150)
                            .Height(150)
                            .Source("ms-appx:///Counter/Assets/logo.png"),
                        new TextBox()
                            .Margin(12)
                            .HorizontalAlignment(HorizontalAlignment.Center)
                            .TextAlignment(Microsoft.UI.Xaml.TextAlignment.Center)
                            .PlaceholderText("Step Size")
                            .Text(x => x.Bind(() => vm.Step).TwoWay()),
                        new TextBlock()
                            .Margin(12)
                            .HorizontalAlignment(HorizontalAlignment.Center)
                            .TextAlignment(Microsoft.UI.Xaml.TextAlignment.Center)
                            .Text(() => vm.Count, txt => $"Counter: {txt}"),
                        new Button()
                            .Margin(12)
                            .HorizontalAlignment(HorizontalAlignment.Center)
                            .Command(() => vm.IncrementCommand)
                            .Content("Increment Counter by Step Size")
                    )
            )
        );
    }
}
