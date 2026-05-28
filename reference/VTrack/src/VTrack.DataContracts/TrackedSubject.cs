namespace VTrack.DataContracts;

/// <summary>
/// A detected and tracked subject in the video
/// </summary>
/// <param name="Id">Unique subject ID (persistent across frames)</param>
/// <param name="Label">Description label for the subject</param>
/// <param name="Color">Hex color for visualization (e.g., "#FF5722")</param>
/// <param name="Confidence">Detection confidence score (0.0 to 1.0)</param>
/// <param name="FirstFrame">First frame where subject appears</param>
/// <param name="LastFrame">Last frame where subject appears</param>
public record TrackedSubject(
    string Id,
    string Label,
    string Color,
    double Confidence,
    int FirstFrame,
    int LastFrame);
