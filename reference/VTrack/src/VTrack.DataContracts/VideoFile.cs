namespace VTrack.DataContracts;

/// <summary>
/// Represents an uploaded video file
/// </summary>
/// <param name="Id">Unique identifier for the video</param>
/// <param name="Name">Original filename</param>
/// <param name="Duration">Video duration in seconds</param>
/// <param name="ThumbnailUrl">URL to video thumbnail</param>
/// <param name="VideoUrl">URL to the video file</param>
/// <param name="UploadedAt">Upload timestamp</param>
public record VideoFile(
    string Id,
    string Name,
    double Duration,
    string? ThumbnailUrl,
    string VideoUrl,
    DateTime UploadedAt);
