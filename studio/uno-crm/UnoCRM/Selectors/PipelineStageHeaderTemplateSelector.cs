using Microsoft.UI.Xaml.Controls;

namespace UnoCRM.Selectors;

/// <summary>
/// Picks the mobile stage header for a pipeline column: one keyed template per stage, each carrying
/// that column's dot colour and count-badge palette as literal <c>{ThemeResource}</c> values — the
/// same five palettes the desktop board writes inline.
/// </summary>
// The stage colours used to travel as resource-key STRINGS on PipelineStage, resolved to Brushes by a
// value converter. That resolves app-wide rather than against the element, so it cannot see the
// element's ActualTheme, and it returns nothing at all in a design-time host. It mattered more here
// than for a dot: the badge pairs a Soft fill with a Deep ink, and those two are near-inverted between
// the themes (a pale cream fill with dark ink in light, a dark fill with bright ink in dark), so a
// mis-resolve renders a badge that is obviously broken rather than slightly off.
//
// The header needs the column's own Name and Count, so this selector takes the whole stage rather than
// just its DealStage. That is safe because the mobile list's items are the records themselves — the
// generated ViewModel builds the bindable list over PipelineStage, not over a proxy of it. Were a
// proxy ever interposed, the type pattern below would stop matching and the headers would render
// EMPTY, silently, since a null template is not an error.
public sealed partial class PipelineStageHeaderTemplateSelector : DataTemplateSelector
{
    public DataTemplate? NewLead { get; set; }

    public DataTemplate? Qualified { get; set; }

    public DataTemplate? Proposal { get; set; }

    public DataTemplate? Negotiation { get; set; }

    public DataTemplate? ClosedWon { get; set; }

    protected override DataTemplate SelectTemplateCore(object item) => (item switch
    {
        PipelineStage { Stage: DealStage.NewLead } => NewLead,
        PipelineStage { Stage: DealStage.Qualified } => Qualified,
        PipelineStage { Stage: DealStage.Proposal } => Proposal,
        PipelineStage { Stage: DealStage.Negotiation } => Negotiation,
        PipelineStage { Stage: DealStage.ClosedWon } => ClosedWon,
        _ => null,
    })!;

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        => SelectTemplateCore(item);
}
