namespace DrawingBackends;

public static class BackendInfo
{
    public static string Name { get; set; } = "Skia";

    public static string Badge => $"Rendering with {Name}";
}
