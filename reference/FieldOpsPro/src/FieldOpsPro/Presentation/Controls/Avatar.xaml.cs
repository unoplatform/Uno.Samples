using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FieldOpsPro.Presentation.Controls;

public sealed partial class Avatar : UserControl
{
    public Avatar()
    {
        this.InitializeComponent();
    }

    public static readonly DependencyProperty InitialsProperty =
        DependencyProperty.Register(nameof(Initials), typeof(string), typeof(Avatar),
            new PropertyMetadata(""));

    public static readonly DependencyProperty AvatarColorProperty =
        DependencyProperty.Register(nameof(AvatarColor), typeof(AvatarColor), typeof(Avatar),
            new PropertyMetadata(AvatarColor.Blue));

    public static readonly DependencyProperty StatusProperty =
        DependencyProperty.Register(nameof(Status), typeof(AgentStatus?), typeof(Avatar),
            new PropertyMetadata(null));

    public static readonly DependencyProperty SizeProperty =
        DependencyProperty.Register(nameof(Size), typeof(double), typeof(Avatar),
            new PropertyMetadata(48.0));

    public string Initials
    {
        get => (string)GetValue(InitialsProperty);
        set => SetValue(InitialsProperty, value);
    }

    public AvatarColor AvatarColor
    {
        get => (AvatarColor)GetValue(AvatarColorProperty);
        set => SetValue(AvatarColorProperty, value);
    }

    public AgentStatus? Status
    {
        get => (AgentStatus?)GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public double Size
    {
        get => (double)GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    /// <summary>Maps avatar Size to a CornerRadius. Bound via x:Bind method-call so it
    /// re-evaluates when Size changes.</summary>
    public CornerRadius CornerRadiusFor(double size)
        => new CornerRadius(size <= 40 ? 10 : size <= 48 ? 12 : 16);
}
