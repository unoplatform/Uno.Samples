namespace MovieStreamApp.Presentation;

/// <summary>
/// Backs <see cref="SocialFeedPage"/>. Static feed data (no interactive members), so it opts out of
/// the bindable generator. Each activity's poster is resolved from the shared catalogue by title, so
/// the movie it references matches the one its card opens.
/// </summary>
[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record SocialFeedModel
{
    public int OnlineFriendsCount => 5;

    public IReadOnlyList<FriendActivity> Activities => new[]
    {
        Activity("f-001", "Priya Sharma", MovieData.PortraitWoman, "is watching", "The Last Horizon", "2m ago",
            "Mind-blowing cinematography! Can't believe the ending.", 14, true),
        Activity("f-002", "Marcus Chen", MovieData.PortraitMan, "just finished", "Crimson Protocol", "18m ago",
            "Action-packed from start to finish. 9/10 would rewatch.", 31, true),
        Activity("f-003", "Sofia Reyes", MovieData.PortraitYoung, "added to watchlist", "Between Worlds", "1h ago",
            "", 8, false),
        Activity("f-004", "Jordan Kato", MovieData.PortraitProfile, "rated 5 stars", "Earth Reborn", "2h ago",
            "Everyone needs to watch this. Absolutely stunning.", 47, false),
        Activity("f-001", "Priya Sharma", MovieData.PortraitWoman, "started watching", "Void Walker", "3h ago",
            "Okay this is actually terrifying... watching with lights on.", 22, true),
        Activity("f-005", "Alex Müller", MovieData.PortraitFriends, "just finished", "Solar Drift", "5h ago",
            "Better than The Last Horizon. The score alone is worth it.", 19, false),
        Activity("f-002", "Marcus Chen", MovieData.PortraitMan, "added to watchlist", "Iron Veil", "Yesterday",
            "", 5, true),
    };

    public IReadOnlyList<FriendSuggestion> SuggestedFriends => new[]
    {
        new FriendSuggestion("f-006", "Nadia Okonkwo", MovieData.PortraitWoman, "12 movies in common"),
        new FriendSuggestion("f-007", "Luca Ferretti", MovieData.PortraitMan, "8 movies in common"),
        new FriendSuggestion("f-008", "Yuna Park", MovieData.PortraitYoung, "Sci-Fi enthusiast"),
    };

    public IReadOnlyList<Movie> TrendingAmongFriends => MovieData.Ids("m-001", "m-009", "m-002");

    private static FriendActivity Activity(
        string id, string name, string avatar, string action, string title, string timeAgo,
        string comment, int likes, bool online) =>
        new(id, name, avatar, action, MovieData.ByTitle(title), timeAgo, comment, likes, online);
}

public partial record FriendSuggestion(
    string FriendId,
    string Name,
    string AvatarUrl,
    string Reason);
