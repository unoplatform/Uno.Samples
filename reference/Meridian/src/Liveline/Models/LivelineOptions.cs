namespace Liveline.Models;

/// <summary>
/// Feature flags controlling which visual elements are rendered.
/// </summary>
public class LivelineOptions
{
    public bool ShowGrid { get; set; } = true;
    public bool ShowBadge { get; set; } = true;
    public bool ShowFill { get; set; } = true;
    public bool ShowMomentum { get; set; } = true;
    public double LerpSpeed { get; set; } = 0.08;
}
