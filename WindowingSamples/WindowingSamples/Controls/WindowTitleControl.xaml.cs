namespace WindowingSamples.Controls;

public sealed partial class WindowTitleControl : UserControl
{
    public WindowTitleControl()
    {
        this.InitializeComponent();
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(WindowTitleControl), new PropertyMetadata(""));
}
