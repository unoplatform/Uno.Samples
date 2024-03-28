namespace TubePlayer.Services;

[Headers("Content-Type: application/json")]
public interface IYoutubeEndpoint
{
    [Get($"/search?part=snippet&maxResults={{maxResult}}&type=video&q={{searchQuery}}&pageToken={{nextPageToken}}")]
    [Headers("Authorization: Bearer")]
    Task<VideoSearchResultData?> SearchVideos(string searchQuery, string nextPageToken, uint maxResult, CancellationToken ct);

    [Get($"/channels?part=snippet,statistics")]
    [Headers("Authorization: Bearer")]
    Task<ChannelSearchResultData?> GetChannels([Query(CollectionFormat.Multi)] string[] id, CancellationToken ct);

    [Get($"/videos?part=contentDetails,id,snippet,statistics")]
    [Headers("Authorization: Bearer")]
    Task<VideoDetailsResultData?> GetVideoDetails([Query(CollectionFormat.Multi)] string[] id, CancellationToken ct);
}
