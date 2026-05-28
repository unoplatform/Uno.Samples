namespace VTrack.DataContracts;

/// <summary>
/// A bounding box detection for a specific frame
/// </summary>
/// <param name="SubjectId">Reference to the tracked subject</param>
/// <param name="Frame">Frame number in the video</param>
/// <param name="X">Left position (normalized 0.0 to 1.0)</param>
/// <param name="Y">Top position (normalized 0.0 to 1.0)</param>
/// <param name="Width">Box width (normalized 0.0 to 1.0)</param>
/// <param name="Height">Box height (normalized 0.0 to 1.0)</param>
/// <param name="Confidence">Detection confidence for this frame</param>
public record BoundingBox(
    string SubjectId,
    int Frame,
    double X,
    double Y,
    double Width,
    double Height,
    double Confidence);
