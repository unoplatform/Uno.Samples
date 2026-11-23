namespace QuoteCraft.Presentation;

/// <summary>The photo a thumbnail opens enlarged (route <c>!PhotoViewer</c>, PRD F14.8).</summary>
public record PhotoViewRequest(string Path);

public partial record PhotoViewerModel(PhotoViewRequest Request)
{
    public string Path => Request.Path;
}
