namespace UnoCRM.Controls;

/// <summary>
/// A single deal tile for the desktop Sales Pipeline board. All cards render at
/// the same height (the title reserves two lines) and the status indicator is
/// pinned top-right in its own column so it never overlaps the company name.
/// Set <see cref="IsWon"/> to swap the colored status dot for a check mark.
/// </summary>
public sealed partial class PipelineCard : UserControl
{
    public PipelineCard()
    {
        this.InitializeComponent();
    }

    public static readonly DependencyProperty CompanyProperty =
        DependencyProperty.Register(nameof(Company), typeof(string), typeof(PipelineCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty AmountProperty =
        DependencyProperty.Register(nameof(Amount), typeof(string), typeof(PipelineCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty OwnerProperty =
        DependencyProperty.Register(nameof(Owner), typeof(string), typeof(PipelineCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty MetaProperty =
        DependencyProperty.Register(nameof(Meta), typeof(string), typeof(PipelineCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty AccentProperty =
        DependencyProperty.Register(nameof(Accent), typeof(Brush), typeof(PipelineCard), new PropertyMetadata(null));

    public static readonly DependencyProperty IsWonProperty =
        DependencyProperty.Register(nameof(IsWon), typeof(bool), typeof(PipelineCard), new PropertyMetadata(false));

    public string Company { get => (string)GetValue(CompanyProperty); set => SetValue(CompanyProperty, value); }
    public string Amount { get => (string)GetValue(AmountProperty); set => SetValue(AmountProperty, value); }
    public string Owner { get => (string)GetValue(OwnerProperty); set => SetValue(OwnerProperty, value); }
    public string Meta { get => (string)GetValue(MetaProperty); set => SetValue(MetaProperty, value); }
    public Brush? Accent { get => (Brush?)GetValue(AccentProperty); set => SetValue(AccentProperty, value); }
    public bool IsWon { get => (bool)GetValue(IsWonProperty); set => SetValue(IsWonProperty, value); }

    // Consumed only by this control's own x:Bind (generated into the same partial class).
    // DotVisibility also gates the muted age meta; CheckVisibility also gates the green "Won" meta.
    private Visibility DotVisibility(bool isWon) => isWon ? Visibility.Collapsed : Visibility.Visible;

    private Visibility CheckVisibility(bool isWon) => isWon ? Visibility.Visible : Visibility.Collapsed;
}
