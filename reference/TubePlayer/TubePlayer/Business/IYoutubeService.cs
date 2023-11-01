namespace TubePlayer.Business;

public interface IYoutubeService
{
    Task<YoutubeVideoSet> SearchVideos(string searchQuery, string nextPageToken, uint maxResult, CancellationToken ct);
}
