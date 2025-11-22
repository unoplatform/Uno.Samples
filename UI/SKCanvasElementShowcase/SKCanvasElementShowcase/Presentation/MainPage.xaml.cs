using Microsoft.UI.Xaml.Controls.Primitives;
using Uno.WinUI.Graphics2DSK;

namespace SKCanvasElementShowcase.Presentation;

public sealed partial class MainPage : Page
{
    public int MaxSampleIndex => SKCanvasElementImpl.SampleCount - 1;

    public MainPage()
    {
        this.InitializeComponent();
        if (SKCanvasElement.IsSupportedOnCurrentPlatform())
        {
            var canvas = new SKCanvasElementImpl();
            border.Child = canvas;
            slider.ValueChanged += (_, e) => canvas.Sample = (int)e.NewValue;
        }
    }
}
