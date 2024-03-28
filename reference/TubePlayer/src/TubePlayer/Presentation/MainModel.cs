using Uno.Extensions.Reactive.Sources;

namespace TubePlayer.Presentation;

public partial record MainModel(IYoutubeService YoutubeService)
{
    public IState<string> SearchTerm => State<string>.Value(this, () => "Uno Platform");

    public IListFeed<YoutubeVideo> VideoSearchResults => SearchTerm
        .Where(searchTerm => searchTerm is { Length: > 0 })
        .SelectPaginatedByCursorAsync(
            firstPage: string.Empty,
            getPage: async (searchTerm, nextPageToken, desiredPageSize, ct) =>
            {
                var videoSet = await YoutubeService.SearchVideos(searchTerm, nextPageToken, desiredPageSize ?? 10, ct);

                return new PageResult<string, YoutubeVideo>(videoSet.Videos, videoSet.NextPageToken);
            });

}
