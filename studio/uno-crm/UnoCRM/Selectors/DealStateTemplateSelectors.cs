using Microsoft.UI.Xaml.Controls;

namespace UnoCRM.Selectors;

// The three state read-outs on the deal detail page — stage, health and dwell time — are drawn by
// one KEYED DataTemplate per state (DataTemplates.xaml), and these selectors pick which one.
//
// Selecting a whole template, rather than converting a palette-key string on the deal into a Brush,
// is what keeps the colors correct: every brush stays a literal {ThemeResource} inside XAML, which
// resolves against the element's own ActualTheme. A key resolved through a value converter is
// resolved app-wide instead, so it can hand back the other theme's brush — and it resolves against
// whichever app owns the resources, which is not this one inside a design-time preview host (that
// is why the old health dot came out invisible in the previews).
//
// Each selector is handed the NARROWEST thing that determines the template. The stage track is a
// pure function of the stage, so it takes the DealStage itself and its templates contain no bindings
// at all — nothing about it can go wrong no matter what sits between the page and the entity.
//
// The health and dwell read-outs need the deal's own text (the health word, the day count, the band
// word), so they are handed the deal. That works because the page's DataContext is DealDetailModel,
// a plain projection record that opts out of the bindable generator, so no generated proxy stands
// between the page and the entity. Give that Model a reactive member and a proxy appears, the type
// patterns below stop matching, and those two read-outs render EMPTY — silently, since a missing
// template is not an error. Hand them the enum and bind the text from the page if that day comes.

/// <summary>
/// Picks the five-step pipeline track for a deal's stage: one keyed template per stage, each
/// raising that stage's step and tinting it with the color the Pipeline board gives that column.
/// Keyed off the <see cref="DealStage"/> alone — the track needs nothing else.
/// </summary>
public sealed partial class DealStageTrackTemplateSelector : DataTemplateSelector
{
    public DataTemplate? NewLead { get; set; }

    public DataTemplate? Qualified { get; set; }

    public DataTemplate? Proposal { get; set; }

    public DataTemplate? Negotiation { get; set; }

    public DataTemplate? ClosedWon { get; set; }

    protected override DataTemplate SelectTemplateCore(object item) => (item switch
    {
        DealStage.NewLead => NewLead,
        DealStage.Qualified => Qualified,
        DealStage.Proposal => Proposal,
        DealStage.Negotiation => Negotiation,
        DealStage.ClosedWon => ClosedWon,
        _ => null,
    })!;

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        => SelectTemplateCore(item);
}

/// <summary>
/// Picks the health mark: a calm round dot, an outlined caution diamond, or a filled alarm
/// triangle in a filled container — an escalation in silhouette, size and weight before color.
/// </summary>
public sealed partial class DealHealthMarkTemplateSelector : DataTemplateSelector
{
    public DataTemplate? Healthy { get; set; }

    public DataTemplate? Watch { get; set; }

    public DataTemplate? AtRisk { get; set; }

    protected override DataTemplate SelectTemplateCore(object item) => (item switch
    {
        Deal { Health: DealHealth.AtRisk } => AtRisk,
        Deal { Health: DealHealth.Watch } => Watch,
        Deal { Health: DealHealth.Healthy } => Healthy,
        _ => null,
    })!;

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        => SelectTemplateCore(item);
}

/// <summary>
/// Picks the dwell read-out. <see cref="Tracked"/> covers the Fresh and Stalling bands (they differ
/// by the gauge's own fill and the band word it shows, not by treatment); <see cref="Stale"/> is the
/// one band that earns color; <see cref="Closed"/> replaces the whole read-out for a closed deal,
/// which is no longer sitting in a stage.
/// </summary>
public sealed partial class DealDwellTemplateSelector : DataTemplateSelector
{
    public DataTemplate? Tracked { get; set; }

    public DataTemplate? Stale { get; set; }

    public DataTemplate? Closed { get; set; }

    protected override DataTemplate SelectTemplateCore(object item) => (item switch
    {
        Deal { AgeBand: DealAgeBand.NotTracked } => Closed,
        Deal { AgeBand: DealAgeBand.Stale } => Stale,
        Deal => Tracked,
        _ => null,
    })!;

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        => SelectTemplateCore(item);
}
