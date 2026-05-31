namespace VTrack.DataContracts;

/// <summary>
/// A natural language query to track subjects in a video
/// </summary>
/// <param name="Id">Unique identifier for the query</param>
/// <param name="VideoId">Reference to the video being analyzed</param>
/// <param name="Query">Natural language description (e.g., "everyone wearing yellow jersey")</param>
/// <param name="CreatedAt">Query submission timestamp</param>
public record TrackingQuery(
    string Id,
    string VideoId,
    string Query,
    DateTime CreatedAt);
