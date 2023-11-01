namespace TubePlayer.Business;

public class YoutubeService : IYoutubeService
{
    private readonly IYoutubeEndpoint _client;

    public YoutubeService(IYoutubeEndpoint client)
    {
        _client = client;
    }

    public async Task<YoutubeVideoSet> SearchVideos(string searchQuery, string nextPageToken, uint maxResult, CancellationToken ct)
    {
        var resultData = await _client.SearchVideos(searchQuery, nextPageToken, maxResult, ct);

        var results = resultData?.Items?.Where(result =>
            !string.IsNullOrWhiteSpace(result.Snippet?.ChannelId)
            && !string.IsNullOrWhiteSpace(result.Id?.VideoId))
            .ToArray();

        if (results?.Any() is not true)
        {
            return YoutubeVideoSet.CreateEmpty();
        }

        var channelIds = results!
            .Select(v => v.Snippet!.ChannelId!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var videoIds = results!
            .Select(v => v.Id!.VideoId!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var asyncDetails = _client.GetVideoDetails(videoIds, ct);
        var asyncChannels = _client.GetChannels(channelIds, ct);
        await Task.WhenAll(asyncDetails, asyncChannels);

        var detailsItems = (await asyncDetails)?.Items;
        var channelsItems = (await asyncChannels)?.Items;

        if (detailsItems is null || channelsItems is null)
        {
            return YoutubeVideoSet.CreateEmpty();
        }

        var detailsResult = detailsItems!
            .Where(detail => !string.IsNullOrWhiteSpace(detail.Id))
            .DistinctBy(detail => detail.Id)
            .ToDictionary(detail => detail.Id!, StringComparer.OrdinalIgnoreCase);

        var channelsResult = channelsItems!
            .Where(channel => !string.IsNullOrWhiteSpace(channel.Id))
            .DistinctBy(channel => channel.Id)
            .ToDictionary(channel => channel.Id!, StringComparer.OrdinalIgnoreCase);

        var videoSet = new List<YoutubeVideo>();
        foreach (var result in results)
        {
            if (channelsResult.TryGetValue(result.Snippet!.ChannelId!, out var channel)
                && detailsResult.TryGetValue(result.Id!.VideoId!, out var details))
            {
                videoSet.Add(new YoutubeVideo(channel, details));
            }
        }

        return new(videoSet.ToImmutableList(), resultData?.NextPageToken ?? string.Empty);
    }
}
