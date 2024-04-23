namespace TubePlayer.Services.Models;

public partial record Format(string? Url, string? QualityLabel);

public partial record StreamingData(List<Format>? Formats);

public partial record YoutubeData(StreamingData? StreamingData);
