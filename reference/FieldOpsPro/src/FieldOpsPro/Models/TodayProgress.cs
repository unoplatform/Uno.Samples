namespace FieldOpsPro.Models;

public record TodayProgress(
    int Completed,
    int Remaining,
    int Total
)
{
    /// <summary>Compact "completed/total" label for the shift header tile.</summary>
    public string TasksSummary => $"{Completed}/{Total}";
}
