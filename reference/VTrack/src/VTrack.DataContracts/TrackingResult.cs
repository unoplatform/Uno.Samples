using System.Collections.Immutable;

namespace VTrack.DataContracts;

/// <summary>
/// Complete tracking results for a video query
/// </summary>
/// <param name="JobId">Reference to the tracking job</param>
/// <param name="VideoId">Reference to the video</param>
/// <param name="Query">The original query text</param>
/// <param name="Subjects">List of tracked subjects</param>
/// <param name="Boxes">All bounding boxes across frames</param>
/// <param name="TotalFrames">Total frames in the video</param>
/// <param name="FrameRate">Video frame rate</param>
public record TrackingResult(
    string JobId,
    string VideoId,
    string Query,
    IImmutableList<TrackedSubject> Subjects,
    IImmutableList<BoundingBox> Boxes,
    int TotalFrames,
    double FrameRate);
