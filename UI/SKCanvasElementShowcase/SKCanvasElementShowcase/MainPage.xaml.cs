namespace SKCanvasElementShowcase;

public sealed partial class MainPage : Page
{
#if DESKTOP
    public int MaxSampleIndex => SKCanvasElementImpl.SampleCount - 1;
#endif

    public MainPage()
    {
        this.InitializeComponent();
    }
}
