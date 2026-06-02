using ClaudeCodeTracker.Presentation.MockData;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ClaudeCodeTracker.Presentation;

public sealed partial class ChartsPage : Page
{
    public static readonly SolidColorPaint DarkLabelPaint = new(new SKColor(0x33, 0x33, 0x33));

    public ChartsPage()
    {
        this.InitializeComponent();
        Root.DataContext = ChartsPageMockData.Data;
    }
}
