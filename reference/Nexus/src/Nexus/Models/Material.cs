namespace Nexus.Models;

public enum StockStatus
{
    InStock,
    LowStock,
    OutOfStock
}

public partial record Material(
    string Id,
    string Name,
    int InStock,
    int MinStock,
    string Unit,
    StockStatus Status
);

public partial record SparePart(
    string Id,
    string Name,
    int InStock,
    int MinStock,
    StockStatus Status
);
