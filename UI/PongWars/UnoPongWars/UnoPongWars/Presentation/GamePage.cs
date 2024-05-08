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
                new Grid().Name(out var grid)
                .Margin(20)
                .RowDefinitions("*,Auto,Auto").Name(out var rows)
                .Children(
                        new ItemsRepeater().Name(out var itemsRepeater)
                            .MinWidth(160)
                            .MinHeight(160)
                            .ItemsSource(() => vm.Cells)
                            .VerticalAlignment(VerticalAlignment.Center)
                            .HorizontalAlignment(HorizontalAlignment.Center)
                            .Layout(new UniformGridLayout()
                                .MinItemWidth(10)
                                .MinItemHeight(10)
                                .MaximumRowsOrColumns(16)
                                .Orientation(Orientation.Vertical)
                                .ItemsStretch(UniformGridLayoutItemsStretch.Uniform))
                            .ItemTemplate<Cell>(cell =>
                                new Grid()
                                .Children(
                                    new Rectangle()
                                        .VerticalAlignment(VerticalAlignment.Stretch)
                                        .HorizontalAlignment(HorizontalAlignment.Stretch)
                                        .Fill(x => x.Binding(() => cell)
                                            .Convert(cell => new SolidColorBrush(CellColor(cell)))),
                                    new Ellipse()
                                        .VerticalAlignment(VerticalAlignment.Stretch)
                                        .HorizontalAlignment(HorizontalAlignment.Stretch)
                                        .Fill(x => x.Binding(() => cell)
                                            .Convert(cell => new SolidColorBrush(PlayerColor(cell))))
                                        .Visibility(x => x.Binding(() => cell)
                                            .Convert(cell => cell.HasBall ? Visibility.Visible : Visibility.Collapsed)))),
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
                            .Value(x => x.Binding(() => vm.Speed).TwoWay())))
            .SizeChanged += async (s, e) =>
            {
                if (itemsRepeater != null)
                {
                    // Calculate the minimum size for the ItemsRepeater to maintain its square aspect ratio.
                    // This ensures that the ItemsRepeater does not extend outside the bounds of the Grid, particularly during
                    // scenarios such as window resizing or device orientation changes, which affect layout dimensions.
                    var size = Math.Min(grid.ActualWidth, rows.RowDefinitions[0].ActualHeight);
                    itemsRepeater.Height = size;
                    itemsRepeater.Width = size;
                    itemsRepeater.UpdateLayout();
                }
            }
        );
    }
}
