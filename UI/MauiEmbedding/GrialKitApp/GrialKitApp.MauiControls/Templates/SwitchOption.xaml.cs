namespace GrialKitApp.MauiControls;

public partial class SwitchOption : ContentView
{
    public static readonly BindableProperty LabelProperty =
        BindableProperty.Create(
            nameof(Label),
            typeof(string),
            typeof(SwitchOption),
            defaultValue: "");

    public string Label
    {
        get { return (string)GetValue(LabelProperty); }
        set { SetValue(LabelProperty, value); }
    }

    public static readonly BindableProperty ValueProperty =
        BindableProperty.Create(
            nameof(Value),
            typeof(bool),
            typeof(SwitchOption),
            defaultValue: true);

    public bool Value
    {
        get { return (bool)GetValue(ValueProperty); }
        set { SetValue(ValueProperty, value); }
    }

    public SwitchOption()
	{
		InitializeComponent();
	}
}
