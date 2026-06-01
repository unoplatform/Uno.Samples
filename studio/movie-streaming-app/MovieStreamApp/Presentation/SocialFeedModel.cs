namespace MovieStreamApp.Presentation;

[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record SocialFeedModel
{
    public int OnlineFriendsCount { get; } = 5;

    public IReadOnlyList<FriendActivity> Activities { get; } = new[]
    {
        new FriendActivity(
            "f-001", "Priya Sharma",
            "https://picsum.photos/seed/woman%20portrait%20casual%20smiling/1024/1024",
            "is watching",
            "The Last Horizon", "Sci-Fi",
            "https://picsum.photos/seed/sci-fi%20space%20movie%20poster/768/1024",
            "2m ago",
            "Mind-blowing cinematography! Can't believe the ending.",
            14, true),

        new FriendActivity(
            "f-002", "Marcus Chen",
            "https://picsum.photos/seed/man%20portrait%20professional%20headshot/1024/1024",
            "just finished",
            "Crimson Protocol", "Action",
            "https://picsum.photos/seed/cinematic%20movie%20poster%20action/768/1024",
            "18m ago",
            "Action-packed from start to finish. 9/10 would rewatch.",
            31, true),

        new FriendActivity(
            "f-003", "Sofia Reyes",
            "https://picsum.photos/seed/young%20person%20smiling%20casual%20portrait/1024/1024",
            "added to watchlist",
            "Between Worlds", "Drama",
            "https://picsum.photos/seed/dark%20thriller%20movie%20poster/768/1024",
            "1h ago",
            "",
            8, false),

        new FriendActivity(
            "f-004", "Jordan Kato",
            "https://picsum.photos/seed/person%20portrait%20smiling%20profile%20photo/1024/1024",
            "rated 5 stars",
            "Earth Reborn", "Documentary",
            "https://picsum.photos/seed/documentary%20nature%20landscape%20film/1280/720",
            "2h ago",
            "Everyone needs to watch this. Absolutely stunning.",
            47, false),

        new FriendActivity(
            "f-001", "Priya Sharma",
            "https://picsum.photos/seed/woman%20portrait%20casual%20smiling/1024/1024",
            "started watching",
            "Void Walker", "Horror",
            "https://picsum.photos/seed/horror%20mystery%20movie%20poster%20dark/768/1024",
            "3h ago",
            "Okay this is actually terrifying... watching with lights on.",
            22, true),

        new FriendActivity(
            "f-005", "Alex Müller",
            "https://picsum.photos/seed/friends%20group%20watching%20movie%20together/1024/1024",
            "just finished",
            "Solar Drift", "Sci-Fi",
            "https://picsum.photos/seed/documentary%20nature%20landscape%20film/1280/720",
            "5h ago",
            "Better than The Last Horizon. The score alone is worth it.",
            19, false),

        new FriendActivity(
            "f-002", "Marcus Chen",
            "https://picsum.photos/seed/man%20portrait%20professional%20headshot/1024/1024",
            "added to watchlist",
            "Iron Veil", "Action",
            "https://picsum.photos/seed/superhero%20action%20blockbuster%20movie/768/1024",
            "Yesterday",
            "",
            5, true),
    };

    public IReadOnlyList<FriendSuggestion> SuggestedFriends { get; } = new[]
    {
        new FriendSuggestion("f-006", "Nadia Okonkwo",
            "https://picsum.photos/seed/woman%20portrait%20casual%20smiling/1024/1024",
            "12 movies in common"),
        new FriendSuggestion("f-007", "Luca Ferretti",
            "https://picsum.photos/seed/man%20portrait%20professional%20headshot/1024/1024",
            "8 movies in common"),
        new FriendSuggestion("f-008", "Yuna Park",
            "https://picsum.photos/seed/young%20person%20smiling%20casual%20portrait/1024/1024",
            "Sci-Fi enthusiast"),
    };

    public IReadOnlyList<Movie> TrendingAmongFriends { get; } = new[]
    {
        new Movie("m-001", "The Last Horizon", "Sci-Fi", "2024", "8.4", "2h 18m",
            "A lone astronaut discovers a signal from the edge of the universe.",
            "https://picsum.photos/seed/sci-fi%20space%20movie%20poster/768/1024", true, false),
        new Movie("m-009", "Earth Reborn", "Documentary", "2024", "9.0", "1h 30m",
            "An astonishing look at how life regenerates in the most hostile environments.",
            "https://picsum.photos/seed/documentary%20nature%20landscape%20film/1280/720", false, true),
        new Movie("m-002", "Crimson Protocol", "Action", "2024", "7.9", "1h 52m",
            "An elite operative uncovers a global conspiracy.",
            "https://picsum.photos/seed/cinematic%20movie%20poster%20action/768/1024", false, false),
    };
}

public partial record FriendSuggestion(
    string FriendId,
    string Name,
    string AvatarUrl,
    string Reason);
