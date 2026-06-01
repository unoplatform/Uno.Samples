namespace MovieStreamApp.Presentation;

public partial record Movie(
    string Id,
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

public partial record FriendActivity(
    string FriendId,
    string FriendName,
    string AvatarUrl,
    string Action,
    string MovieTitle,
    string MovieGenre,
    string MovieImageUrl,
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
