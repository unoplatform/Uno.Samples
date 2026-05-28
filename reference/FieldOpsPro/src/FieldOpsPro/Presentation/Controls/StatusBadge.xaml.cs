using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FieldOpsPro.Presentation.Controls;

public sealed partial class StatusBadge : UserControl
{
    public StatusBadge()
    {
        this.InitializeComponent();
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(StatusBadge),
            new PropertyMetadata(""));

    public static readonly DependencyProperty BadgeTypeProperty =
        DependencyProperty.Register(nameof(BadgeType), typeof(BadgeType), typeof(StatusBadge),
            new PropertyMetadata(BadgeType.Default));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public BadgeType BadgeType
    {
        get => (BadgeType)GetValue(BadgeTypeProperty);
        set => SetValue(BadgeTypeProperty, value);
    }
}

public enum BadgeType
{
    Default,
    Primary,
    Success,
    Warning,
    Danger,
    Info,
    Urgent,
    Pending,
    Completed,
    InProgress
}
