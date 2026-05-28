namespace VTrack.DataContracts;

/// <summary>
/// Status of a video tracking processing job
/// </summary>
public enum JobStatus
{
    Pending,
    Processing,
    Completed,
    Failed
}

/// <summary>
/// A video tracking job with status information
/// </summary>
/// <param name="Id">Unique job identifier</param>
/// <param name="VideoId">Reference to the video</param>
/// <param name="QueryId">Reference to the tracking query</param>
/// <param name="Status">Current job status</param>
/// <param name="Progress">Processing progress (0.0 to 1.0)</param>
/// <param name="ErrorMessage">Error details if failed</param>
/// <param name="CreatedAt">Job creation timestamp</param>
/// <param name="CompletedAt">Job completion timestamp</param>
public record TrackingJob(
    string Id,
    string VideoId,
    string QueryId,
    JobStatus Status,
    double Progress,
    string? ErrorMessage,
    DateTime CreatedAt,
    DateTime? CompletedAt);
