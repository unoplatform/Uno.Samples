namespace Caffe.Models;

public enum GrindLevel
{
    Fine = 0,
    Medium = 1,
    Coarse = 2
}

public static class GrindLevelExtensions
{
    public static string ToLabel(this GrindLevel level) => level switch
    {
        GrindLevel.Fine => "Fine",
        GrindLevel.Medium => "Medium",
        GrindLevel.Coarse => "Coarse",
        _ => "Medium"
    };

    public static string ToHint(this GrindLevel level) => level switch
    {
        GrindLevel.Fine => "Slower",
        GrindLevel.Medium => "Balanced",
        GrindLevel.Coarse => "Faster",
        _ => "Balanced"
    };

    public static string ToAbbreviation(this GrindLevel level) => level switch
    {
        GrindLevel.Fine => "F",
        GrindLevel.Medium => "M",
        GrindLevel.Coarse => "C",
        _ => "M"
    };
}
