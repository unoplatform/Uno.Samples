namespace TubePlayer.Business;

public class YoutubeService(IYoutubeEndpoint client, IYoutubePlayerEndpoint playerClient) : IYoutubeService
{
    public async Task<YoutubeVideoSet> SearchVideos(string searchQuery, string nextPageToken, uint maxResult, CancellationToken ct)
    {
        var resultData = await client.SearchVideos(searchQuery, nextPageToken, maxResult, ct);

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

        var asyncDetails = client.GetVideoDetails(videoIds, ct);
        var asyncChannels = client.GetChannels(channelIds, ct);
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


    public async Task<string?> GetVideoSourceUrl(string videoId, CancellationToken ct)
    {
        var streamVideo = $$"""
                {
                    "videoId": "{{videoId}}",
                    "context": {
                        "client": {
                            "clientName": "ANDROID_TESTSUITE",
                            "clientVersion": "1.9",
                            "androidSdkVersion": 30,
                            "hl": "en",
                            "gl": "US",
                            "utcOffsetMinutes": 0
                        }
                    }
                }
                """;

        // Get the available stream data
        var streamData = await playerClient.GetStreamData(streamVideo, ct);

        // Get the video stream with the highest video quality
        var streamWithHighestVideoQuality = streamData.Content?.
                                                        StreamingData?
                                                        .Formats?
                                                        .OrderByDescending(s => s.QualityLabel)
                                                        .FirstOrDefault();

        // Get the stream URL
        var streamUrl = streamWithHighestVideoQuality?.Url;

        return streamUrl;
    }
}
