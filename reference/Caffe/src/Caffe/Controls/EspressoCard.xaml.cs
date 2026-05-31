using Caffe.Models;

namespace Caffe.Controls;

public sealed partial class EspressoCard : UserControl
{
    public static readonly DependencyProperty EspressoProperty =
        DependencyProperty.Register(
            nameof(Espresso),
            typeof(EspressoItem),
            typeof(EspressoCard),
            new PropertyMetadata(null));

    public static readonly DependencyProperty IsSelectedProperty =
        DependencyProperty.Register(
            nameof(IsSelected),
            typeof(bool),
            typeof(EspressoCard),
            new PropertyMetadata(false));

    public EspressoItem? Espresso
    {
        get => (EspressoItem?)GetValue(EspressoProperty);
        set => SetValue(EspressoProperty, value);
    }

    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public EspressoCard()
    {
        this.InitializeComponent();
    }

    // x:Bind function bindings (see EspressoCard.xaml).
    private string A11yName(EspressoItem? item) =>
        item is null ? string.Empty : $"{item.Name} espresso, {item.VolumeDisplay}";

    private Brush SelectionBorderBrush(bool isSelected) =>
        (Brush)Application.Current.Resources[isSelected ? "CaffePrimaryBrush" : "CaffeBorderBrush"];
}
