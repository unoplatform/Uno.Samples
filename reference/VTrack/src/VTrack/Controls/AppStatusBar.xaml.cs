namespace VTrack.Controls;

public sealed partial class AppStatusBar : UserControl
{
    public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
        nameof(State), typeof(string), typeof(AppStatusBar),
        new PropertyMetadata("READY", (d, e) => ((AppStatusBar)d).StateLabel.Text = (string)e.NewValue));

    public static readonly DependencyProperty MediaProperty = DependencyProperty.Register(
        nameof(Media), typeof(string), typeof(AppStatusBar),
        new PropertyMetadata("no recording loaded", (d, e) => ((AppStatusBar)d).MediaLabel.Text = (string)e.NewValue));

    public static readonly DependencyProperty FrameProperty = DependencyProperty.Register(
        nameof(Frame), typeof(string), typeof(AppStatusBar),
        new PropertyMetadata("F 000 / 000", (d, e) => ((AppStatusBar)d).FrameLabel.Text = (string)e.NewValue));

    public static readonly DependencyProperty EngineProperty = DependencyProperty.Register(
        nameof(Engine), typeof(string), typeof(AppStatusBar),
        new PropertyMetadata("OpenCL · GPU", (d, e) => ((AppStatusBar)d).EngineLabel.Text = (string)e.NewValue));

    public string State { get => (string)GetValue(StateProperty); set => SetValue(StateProperty, value); }
    public string Media { get => (string)GetValue(MediaProperty); set => SetValue(MediaProperty, value); }
    public string Frame { get => (string)GetValue(FrameProperty); set => SetValue(FrameProperty, value); }
    public string Engine { get => (string)GetValue(EngineProperty); set => SetValue(EngineProperty, value); }

    public AppStatusBar()
    {
        this.InitializeComponent();
    }
}
