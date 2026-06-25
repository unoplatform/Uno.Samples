namespace ClaudeCodeTracker.Presentation;

[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record SessionsModel
{
    public string TotalCountDisplay => $"{Fmt.Count(SampleData.TotalSessions)} total sessions";

    /// <summary>Model filter labels ("All", "Opus", …) shown as a ChipGroup.</summary>
    public IReadOnlyList<string> FilterOptions => SampleData.ModelFilters;

    /// <summary>The full session list; the page applies search + model filtering on top.</summary>
    public IReadOnlyList<SessionEntry> Sessions => SampleData.Sessions;
}
