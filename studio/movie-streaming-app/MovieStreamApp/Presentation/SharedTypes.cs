namespace MovieStreamApp.Presentation;

// Domain records are immutable (MVUX requirement). A record that flows through an IListState<T>
// (the watchlist) declares a [property: Key] on its identity so add/remove/selection match by
// identity, not reference — key equality is auto-generated for partial records.

public partial record Movie(
    [property: global::Uno.Extensions.Equality.Key] string Id,
    string Title,
    string Genre,
    string Year,
    string Rating,
    string Duration,
    string Description,
    string ImageUrl,
    bool IsFeatured,
    bool IsNew);

public partial record CastMember(
    string Name,
    string Role);

public partial record CastMemberDetail(
    string Name,
    string Role,
    string ImageUrl);

public partial record FriendActivity(
    string FriendId,
    string FriendName,
    string AvatarUrl,
    string Action,
    Movie Movie,
    string TimeAgo,
    string Comment,
    int LikeCount,
    bool IsOnline);

public partial record Review(
    string ReviewId,
    string AuthorName,
    string AvatarUrl,
    int Stars,
    string Body,
    string TimeAgo,
    int HelpfulCount);
