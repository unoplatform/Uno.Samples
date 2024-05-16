namespace UnoPongWars.Presentation;

public sealed partial class GamePage : Page
{
    private readonly Color Blue = Color.FromArgb(255, 27, 154, 249);
    private readonly Color Green = Color.FromArgb(255, 107, 227, 173);

    public object ViewModel { get; set; }

    public Color PlayerColor(Cell cell) => cell.Player == 0 ? Blue : Green;
    public Color CellColor(Cell cell) => cell.Player == 0 ? Green : Blue;

    public GamePage()
    {
        this.DataContext<BindableGameModel>((page, vm) => page
            .NavigationCacheMode(NavigationCacheMode.Required)
            .SafeArea(SafeArea.InsetMask.All)
            .Background(Theme.Brushes.Background.Default)
            .Content(
                new Grid()
                    .Margin(20)
                    .RowDefinitions("*,Auto,Auto")
                    .Children(
                        new Viewbox()
                            .HorizontalAlignment(HorizontalAlignment.Center)
                            .Child(
                                new ItemsRepeater()
                                    .ItemsSource(() => vm.Cells)
                                    .VerticalAlignment(VerticalAlignment.Center)
                                    .Width(160)
                                    .Height(160)
                                    .Layout(
                                        new UniformGridLayout()
                                            .Orientation(Orientation.Vertical)
                                            .MaximumRowsOrColumns(16))
                                    .ItemTemplate<Cell>(cell => CellTemplate(cell))),
                                new TextBlock()
                                    .Text(() => vm.Score)
                                    .HorizontalAlignment(HorizontalAlignment.Center)
                                    .Style(Theme.TextBlock.Styles.HeadlineSmall)
                                    .Margin(10)
                                    .Grid(row: 1),
                                new Slider()
                                    .MaxWidth(400)
                                    .Margin(16)
                                    .Maximum(1000)

// Adjusting minimum slider value for Android to address the UI layer performance concerns specific to this platform
#if __ANDROID__
                                    .Minimum(100)
#else
                                    .Minimum(10)
#endif
                                    .Grid(row: 2)
                                    .Value(x => x.Binding(() => vm.Speed).TwoWay())
            )));
    }

    private Grid CellTemplate(Cell cell)
        => new Grid()
            .Children(
                new Rectangle()
                    .Width(10)
                    .Height(10)
                    .Fill(() => cell, cell => new SolidColorBrush(CellColor(cell))),
                new Ellipse()
                    .Width(10)
                    .Height(10)
                    .Fill(() => cell, cell => new SolidColorBrush(PlayerColor(cell)))
                    .Visibility(() => cell, cell => cell.HasBall ? Visibility.Visible : Visibility.Collapsed));
}
