namespace TubePlayer.Services;

[Headers(
    "Content-Type: application/json",
    "User-Agent", "com.google.android.youtube/17.36.4 (Linux; U; Android 12; GB) gzip")]
public interface IYoutubePlayerEndpoint
{
    [Post("/player")]
    Task<ApiResponse<YoutubeData>> GetStreamData([Body] string data, CancellationToken cancellationToken = default);
}
