namespace Liveline.Models;

/// <summary>
/// Theme configuration derived from a single hex color.
/// </summary>
public class LivelineTheme
{
    /// <summary>
    /// Primary hex color (e.g. "#4CAF50"). The full palette is derived from this.
    /// </summary>
    public string Color { get; set; } = "#4CAF50";

    /// <summary>
    /// True for dark background, false for light.
    /// </summary>
    public bool IsDark { get; set; }
}
