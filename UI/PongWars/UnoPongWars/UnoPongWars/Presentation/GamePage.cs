namespace UnoPongWars.Presentation;

public sealed partial class GamePage : Page
{
    private readonly Color UnoBleu = Color.FromArgb(255, 27, 154, 249);
    private readonly Color UnoVert = Color.FromArgb(255, 107, 227, 173);

    public object ViewModel { get; set; }

    public Color PlayerColor(Cell cell) => cell.Player == 0 ? UnoBleu : UnoVert;
    public Color CellColor(Cell cell) => cell.Player == 0 ? UnoVert : UnoBleu;

    public GamePage()
    {
        this.DataContext<BindableGameModel>((page, vm) => page
            .NavigationCacheMode(NavigationCacheMode.Required)
            .SafeArea(SafeArea.InsetMask.All)
            .Background(Theme.Brushes.Background.Default)
            .Content(
                new Grid()
                .RowDefinitions("*,auto,auto")
                .Children(
                     new Viewbox()
                     .HorizontalAlignment(HorizontalAlignment.Center)
                     .Child(
                        new ItemsRepeater()
                            .ItemsSource(() => vm.Cells)
                            .VerticalAlignment(VerticalAlignment.Center)
                            .Width(160)
                            .Height(160)
                            .Layout(new UniformGridLayout()
                                .MaximumRowsOrColumns(16))
                            .ItemTemplate<Cell>(cell =>
                                new Grid()
                                .Children(
                                    new Rectangle()
                                        .Width(10)
                                        .Height(10)
                                        .Fill(x => x.Binding(() => cell)
                                            .Convert(cell => new SolidColorBrush(CellColor(cell)))),
                                    new Ellipse()
                                        .Width(10)
                                        .Height(10)
                                        .Fill(x => x.Binding(() => cell)
                                            .Convert(cell => new SolidColorBrush(PlayerColor(cell))))
                                        .Visibility(x => x.Binding(() => cell)
                                            .Convert(cell => cell.HasBall ? Visibility.Visible : Visibility.Collapsed))))),
                        new TextBlock()
                            .Text(x => x.Binding(() => vm.Score))
                            .HorizontalAlignment(HorizontalAlignment.Center)
                            .Style(Theme.TextBlock.Styles.HeadlineSmall)
                            .Margin(10)
                            .Grid(row: 1),
                        new Slider()
                            .MaxWidth(400)
                            .Margin(16)
                            .Maximum(1000)
#if __ANDROID__
                            .Minimum(100)
#else
                            .Minimum(10)
#endif
                            .Grid(row: 2)
                            .Value(x => x.Binding(() => vm.Speed).TwoWay())
            )));
    }
}
