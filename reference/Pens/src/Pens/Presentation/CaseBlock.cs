namespace Pens.Presentation;

public record CaseBlock(int Index, bool IsConsumed)
{
    // C2: spoken state for screen readers (state is otherwise conveyed by fill colour only).
    public string AccessibleName => $"Case {Index + 1}, {(IsConsumed ? "consumed" : "remaining")}";
}
