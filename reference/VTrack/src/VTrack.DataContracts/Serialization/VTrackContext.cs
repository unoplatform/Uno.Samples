using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace VTrack.DataContracts.Serialization;

[JsonSerializable(typeof(VideoFile))]
[JsonSerializable(typeof(VideoFile[]))]
[JsonSerializable(typeof(IImmutableList<VideoFile>))]
[JsonSerializable(typeof(ImmutableList<VideoFile>))]
[JsonSerializable(typeof(TrackingQuery))]
[JsonSerializable(typeof(TrackedSubject))]
[JsonSerializable(typeof(TrackedSubject[]))]
[JsonSerializable(typeof(IImmutableList<TrackedSubject>))]
[JsonSerializable(typeof(ImmutableList<TrackedSubject>))]
[JsonSerializable(typeof(BoundingBox))]
[JsonSerializable(typeof(BoundingBox[]))]
[JsonSerializable(typeof(IImmutableList<BoundingBox>))]
[JsonSerializable(typeof(ImmutableList<BoundingBox>))]
[JsonSerializable(typeof(TrackingJob))]
[JsonSerializable(typeof(TrackingResult))]
[JsonSerializable(typeof(StartTrackingRequest))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class VTrackContext : JsonSerializerContext
{
}

public record StartTrackingRequest(string VideoId, string Query);
