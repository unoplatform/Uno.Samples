using Windows.Media.Core;

namespace TubePlayer.Presentation;

public partial record VideoDetailsModel(YoutubeVideo Video, IYoutubeService YoutubeService)
{
    public IFeed<MediaSource> VideoSource => Feed.Async(GetVideoSource);

    private async ValueTask<MediaSource> GetVideoSource(CancellationToken ct)
    {
        var streamUrl = await YoutubeService.GetVideoSourceUrl(Video?.Id ?? string.Empty, ct)
            ?? throw new InvalidOperationException("Input stream collection is empty.");

        // Return the MediaSource using the stream URL
        return MediaSource.CreateFromUri(new Uri(streamUrl));
    }
}
