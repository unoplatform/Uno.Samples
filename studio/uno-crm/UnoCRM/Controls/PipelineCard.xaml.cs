namespace UnoCRM.Controls;

/// <summary>
/// A single deal tile for the desktop Sales Pipeline board. All cards render at
/// the same height (the title reserves two lines) and the status indicator is
/// pinned top-right in its own column so it never overlaps the company name.
/// <see cref="Health"/> picks which of the three escalating marks shows; setting
/// <see cref="IsWon"/> replaces it with a check, since health no longer applies to
/// a closed deal.
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

    public static readonly DependencyProperty HealthProperty =
        DependencyProperty.Register(nameof(Health), typeof(DealHealth), typeof(PipelineCard), new PropertyMetadata(DealHealth.Healthy));

    public static readonly DependencyProperty IsWonProperty =
        DependencyProperty.Register(nameof(IsWon), typeof(bool), typeof(PipelineCard), new PropertyMetadata(false));

    public string Company { get => (string)GetValue(CompanyProperty); set => SetValue(CompanyProperty, value); }
    public string Amount { get => (string)GetValue(AmountProperty); set => SetValue(AmountProperty, value); }
    public string Owner { get => (string)GetValue(OwnerProperty); set => SetValue(OwnerProperty, value); }
    public string Meta { get => (string)GetValue(MetaProperty); set => SetValue(MetaProperty, value); }
    public DealHealth Health { get => (DealHealth)GetValue(HealthProperty); set => SetValue(HealthProperty, value); }
    public bool IsWon { get => (bool)GetValue(IsWonProperty); set => SetValue(IsWonProperty, value); }

    // One mark at a time: a won deal shows the check and no health mark at all, and an open deal
    // shows exactly the mark matching its health. One function per mark because x:Bind cannot name
    // an enum member in an argument list, and passing the state in as a property would have to be
    // an instance member for x:Bind to reach it.
    private Visibility HealthyVisibility(bool isWon, DealHealth health) => MarkVisibility(isWon, health, DealHealth.Healthy);

    private Visibility WatchVisibility(bool isWon, DealHealth health) => MarkVisibility(isWon, health, DealHealth.Watch);

    private Visibility AtRiskVisibility(bool isWon, DealHealth health) => MarkVisibility(isWon, health, DealHealth.AtRisk);

    private static Visibility MarkVisibility(bool isWon, DealHealth health, DealHealth mark) =>
        !isWon && health == mark ? Visibility.Visible : Visibility.Collapsed;

    // DotVisibility also gates the muted age meta; CheckVisibility also gates the green "Won" meta.
    private Visibility DotVisibility(bool isWon) => isWon ? Visibility.Collapsed : Visibility.Visible;

    private Visibility CheckVisibility(bool isWon) => isWon ? Visibility.Visible : Visibility.Collapsed;
}
