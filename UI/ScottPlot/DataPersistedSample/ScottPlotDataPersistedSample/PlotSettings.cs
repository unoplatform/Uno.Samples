using SQLite;

namespace ScottPlotDataPersistedSample;

public class PlotSettings
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Store the last used plot type
    public string? LastUsedPlotType { get; set; }
}
