namespace TubePlayer.Business;

public interface IYoutubeService
{
    Task<string?> GetVideoSourceUrl(string userId, CancellationToken ct);

    Task<YoutubeVideoSet> SearchVideos(string searchQuery, string nextPageToken, uint maxResult, CancellationToken ct);
}
