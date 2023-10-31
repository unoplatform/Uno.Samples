using System.Diagnostics.CodeAnalysis;

namespace TubePlayer.Business;

public class YoutubeServiceMock : IYoutubeService
{
    private readonly VideoDetailsResultData _details;
    private readonly IDictionary<string, ChannelData> _channels;

    public YoutubeServiceMock(ISerializer serializer)
    {
        _details = serializer.FromString<VideoDetailsResultData>(YoutubeServiceMockData.DetailsData)!;

        var channelsData = serializer.FromString<ChannelSearchResultData>(YoutubeServiceMockData.ChannelData)!;
        _channels = channelsData.Items!.ToDictionary(channel => channel.Id!, StringComparer.OrdinalIgnoreCase);
    }

    public Task<YoutubeVideoSet> SearchVideos(string searchQuery, string nextPageToken, uint maxResult, CancellationToken ct)
    {
        var filtered = _details
            .Items!
            .Where(detail =>
                detail.Snippet!.Title!.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));

        var videos = filtered
                    .Select(detail => new YoutubeVideo(_channels[detail.Snippet!.ChannelId!], detail))
                    .ToImmutableList();

        var result = new YoutubeVideoSet(videos, NextPageToken: string.Empty);

        return Task.FromResult(result);
    }
}
