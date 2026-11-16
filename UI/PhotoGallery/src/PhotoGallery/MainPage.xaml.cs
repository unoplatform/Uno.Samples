using Microsoft.UI.Xaml.Controls.Primitives;

namespace PhotoGallery;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();

        JustificationCombo.ItemsSource = Enum.GetValues<LinedFlowLayoutItemsJustification>();
        JustificationCombo.SelectedItem = LinedFlowLayoutItemsJustification.Start;
        StretchCombo.ItemsSource = new[] { LinedFlowLayoutItemsStretch.None, LinedFlowLayoutItemsStretch.Fill };
        StretchCombo.SelectedItem = LinedFlowLayoutItemsStretch.Fill;
        Gallery.LayoutUpdated += (_, _) => ActualLineHeightText.Text = $"{GalleryLayout.ActualLineHeight:0.#} px";
    }

    public IReadOnlyList<Photo> Photos { get; } = Photo.CreateSet();

    private void OnItemsInfoRequested(LinedFlowLayout sender, LinedFlowLayoutItemsInfoRequestedEventArgs args)
    {
        var ratios = new double[args.ItemsRangeRequestedLength];
        for (int i = 0; i < ratios.Length; i++)
        {
            ratios[i] = Photos[args.ItemsRangeStartIndex + i].AspectRatio;
        }

        args.SetDesiredAspectRatios(ratios);
    }

    private void OnLineHeightChanged(object sender, RangeBaseValueChangedEventArgs e) => GalleryLayout.LineHeight = e.NewValue;

    private void OnLineSpacingChanged(object sender, RangeBaseValueChangedEventArgs e) => GalleryLayout.LineSpacing = e.NewValue;

    private void OnItemSpacingChanged(object sender, RangeBaseValueChangedEventArgs e) => GalleryLayout.MinItemSpacing = e.NewValue;

    private void OnJustificationChanged(object sender, SelectionChangedEventArgs e)
    {
        if (JustificationCombo.SelectedItem is LinedFlowLayoutItemsJustification value)
        {
            GalleryLayout.ItemsJustification = value;
        }
    }

    private void OnStretchChanged(object sender, SelectionChangedEventArgs e)
    {
        if (StretchCombo.SelectedItem is LinedFlowLayoutItemsStretch value)
        {
            GalleryLayout.ItemsStretch = value;
        }
    }
}
